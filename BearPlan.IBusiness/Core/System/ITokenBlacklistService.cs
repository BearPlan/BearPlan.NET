using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// Token黑名单
/// </summary>
public interface ITokenBlacklistService : IBaseServices<TokenBlacklist>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<TokenBlacklistDTO>> GetPageAsync(TokenBlacklistParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<TokenBlacklistInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateTokenBlacklistParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateTokenBlacklistParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
}
