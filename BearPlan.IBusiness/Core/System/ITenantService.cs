using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// 租户接口
/// </summary>
public interface ITenantService : IBaseServices<Tenant>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<TenantDTO>> GetPageAsync(TenantParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<TenantInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateTenantParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateTenantParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 查询全部
    /// </summary>
    Task<List<TenantDTO>> QueryAllAsync();
    #endregion
}
