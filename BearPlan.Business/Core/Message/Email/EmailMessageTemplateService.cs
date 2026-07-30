using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.Message.Email;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.Message.Email;

/// <summary>
/// 邮件消息模板服务
/// </summary>
public class EmailMessageTemplateService : BaseServices<EmailMessageTemplate>, IEmailMessageTemplateService
{
    #region CRUD
    public async Task<PagedResults<EmailMessageTemplateDTO>> GetPageAsync(EmailMessageTemplateParam param)
    {
        var page = await GetIQueryable().Select(x => new EmailMessageTemplateDTO
        {
            Id = x.Id,
            Name = x.Name,
            Subject = x.Subject,
            Body = x.Body,
            EmailAccountId = x.EmailAccountId,
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    public async Task<EmailMessageTemplateInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<EmailMessageTemplateInfo>().FirstAsync();
        return entity;
    }

    public async Task<long> AddAsync(UpdateEmailMessageTemplateParam param)
    {
        var model = App.Mapper.MapTo<EmailMessageTemplate>(param);
        await AddAsync(model);
        return model.Id;
    }

    public async Task<long> UpdateAsync(UpdateEmailMessageTemplateParam param)
    {
        var model = App.Mapper.MapTo<EmailMessageTemplate>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
}
