using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Message.Email;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.Message.Email;

/// <summary>
/// 邮箱账户接口
/// </summary>
public interface IEmailAccountService : IBaseServices<EmailAccount>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<EmailAccountDTO>> GetPageAsync(EmailAccountParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<EmailAccountInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateEmailAccountParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateEmailAccountParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 查询所有
    /// </summary>
    Task<List<EmailAccountDTO>> QueryAllAsync();
    #endregion
}
