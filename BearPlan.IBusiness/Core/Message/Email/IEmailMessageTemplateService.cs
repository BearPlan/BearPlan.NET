using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Message.Email;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.Message.Email;

/// <summary>
/// 邮件消息模板接口
/// </summary>
public interface IEmailMessageTemplateService : IBaseServices<EmailMessageTemplate>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<EmailMessageTemplateDTO>> GetPageAsync(EmailMessageTemplateParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<EmailMessageTemplateInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateEmailMessageTemplateParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateEmailMessageTemplateParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
}
