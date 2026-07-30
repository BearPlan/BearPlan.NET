using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Helper;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Core.Utils;
using BearPlan.Entity.Core.Message.Email;
using BearPlan.Entity.Core.Queued;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.Extensions.Logging;
using SqlSugar;
using BearPlan.Common.Global;

namespace BearPlan.Business.Core.Queued;

/// <summary>
/// 邮件队列服务
/// </summary>
public class QueuedEmailService : BaseServices<QueuedEmail>, IQueuedEmailService
{
    #region 字段
    private readonly IEmailMessageTemplateService _emailMessageTemplateService;
    private readonly IEmailAccountService _emailAccountService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<QueuedEmailService> _logger;
    #endregion

    #region 构造函数
    public QueuedEmailService(
        IEmailMessageTemplateService emailMessageTemplateService,
        IEmailAccountService emailAccountService,
        IEmailSender emailSender,
        ILogger<QueuedEmailService> logger)
    {
        _emailMessageTemplateService = emailMessageTemplateService;
        _emailAccountService = emailAccountService;
        _emailSender = emailSender;
        _logger = logger;
    }
    #endregion

    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<QueuedEmailDTO>> GetPageAsync(QueuedEmailParam param)
    {
        var page = await GetIQueryable().Select(x => new QueuedEmailDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<QueuedEmailInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<QueuedEmailInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateQueuedEmailParam param)
    {
        var emailAccount = await _emailAccountService.GetIQueryable(x => x.Id == param.EmailAccountId)
            .SingleAsync();
        if (emailAccount == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        param.From = emailAccount.Email;
        param.FromName = emailAccount.DisplayName;
        var queuedEmail = App.Mapper.MapTo<QueuedEmail>(param);
        await AddAsync(queuedEmail);
        return queuedEmail.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateQueuedEmailParam param)
    {
        if (!await GetIQueryable(x => x.Id == param.Id).AnyAsync())
        {
            throw new BusException(ValidationError.NotExist());
        }

        var emailAccount = await _emailAccountService.GetIQueryable(x => x.Id == param.EmailAccountId)
            .SingleAsync();
        if (emailAccount == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        param.From = emailAccount.Email;
        param.FromName = emailAccount.DisplayName;
        var queuedEmail = App.Mapper.MapTo<QueuedEmail>(param);
        await UpdateAsync(queuedEmail);
        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        var emailList = await GetIQueryable(x => ids.Contains(x.Id)).ToListAsync();
        if (emailList.Count < 1)
        {
            throw new BusException(ValidationError.NotExist());
        }

        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion

    #region 扩展
    /// <summary>
    /// 变更邮箱验证码
    /// </summary>
    public async Task ResetEmailCodeAsync(string emailAddress, string messageTemplateName)
    {
        var emailMessageTemplate =
            await _emailMessageTemplateService.GetIQueryable(x => x.Name == messageTemplateName).FirstAsync();
        if (emailMessageTemplate == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        var emailAccount = await _emailAccountService.GetIQueryable(x => x.Id == emailMessageTemplate.EmailAccountId)
            .SingleAsync();
        if (emailAccount == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        // 生成6位随机码
        var captcha = SixLaborsImageHelper.BuilEmailCaptcha(6);

        var queuedEmail = new QueuedEmail
        {
            From = emailAccount.Email,
            FromName = emailAccount.DisplayName,
            To = emailAddress,
            Priority = QueuedEmailPriority.High,
            Bcc = emailMessageTemplate.BccEmailAddresses,
            Subject = emailMessageTemplate.Subject,
            Body = emailMessageTemplate.Body.Replace("%captcha%", captcha),
            SentTries = 1,
            EmailAccountId = emailAccount.Id
        };

        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.EmailCaptcha +
                                    queuedEmail.To.ToMd5String());
        var isTrue = await App.Cache.SetAsync(
            GlobalConstants.CachePrefix.EmailCaptcha + queuedEmail.To.ToMd5String16(), captcha,
            TimeSpan.FromMinutes(5), null);

        if (isTrue)
        {
            var bcc = string.IsNullOrWhiteSpace(queuedEmail.Bcc)
                ? null
                : queuedEmail.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var cc = string.IsNullOrWhiteSpace(queuedEmail.Cc)
                ? null
                : queuedEmail.Cc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                var sendAccount = await _emailAccountService.GetIQueryable(x => x.Id == queuedEmail.EmailAccountId)
                    .FirstAsync();
                isTrue = await _emailSender.SendEmailAsync(
                    sendAccount,
                    queuedEmail.Subject,
                    queuedEmail.Body,
                    queuedEmail.From,
                    queuedEmail.FromName,
                    queuedEmail.To,
                    queuedEmail.ToName,
                    queuedEmail.ReplyTo,
                    queuedEmail.ReplyToName,
                    bcc,
                    cc);
                queuedEmail.IsSend = isTrue;
                if (isTrue)
                {
                    queuedEmail.SendTime = DateTime.Now;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError("Error sending e-mail. {Message}", exc.Message);
                isTrue = false;
            }
            finally
            {
                try
                {
                    await AddAsync(queuedEmail);
                }
                catch
                {
                    // ignored
                }
            }
        }

        if (!isTrue)
        {
            throw new BusException("邮件发送失败");
        }
    }

    /// <summary>
    /// 查询待发送邮件
    /// </summary>
    public async Task<List<QueuedEmailInfo>> QueryToSendMailAsync()
    {
        return await GetIQueryable(x => !x.IsSend && x.SentTries < 3)
            .Select<QueuedEmailInfo>().ToListAsync();
    }

    /// <summary>
    /// 更新发送次数
    /// </summary>
    public async Task UpdateTriesAsync(QueuedEmail queuedEmail)
    {
        await UpdateAsync(queuedEmail);
    }
    #endregion
}
