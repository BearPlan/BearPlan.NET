using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Queued;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.Queued;

/// <summary>
/// 邮件队列接口
/// </summary>
public interface IQueuedEmailService : IBaseServices<QueuedEmail>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<QueuedEmailDTO>> GetPageAsync(QueuedEmailParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<QueuedEmailInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateQueuedEmailParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateQueuedEmailParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion

    #region 扩展
    /// <summary>
    /// 重置邮箱验证码
    /// </summary>
    Task ResetEmailCodeAsync(string emailAddress, string messageTemplateName);

    /// <summary>
    /// 查询待发送邮件
    /// </summary>
    Task<List<QueuedEmailInfo>> QueryToSendMailAsync();

    /// <summary>
    /// 更新发送次数
    /// </summary>
    Task UpdateTriesAsync(QueuedEmail queuedEmail);
    #endregion
}
