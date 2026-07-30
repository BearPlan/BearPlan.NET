using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// apis 接口
/// </summary>
public interface IApisService : IBaseServices<Apis>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<ApisDTO>> GetPageAsync(ApisParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<ApisInfo> GetInfoAsync(Guid id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<Guid> AddAsync(UpdateApisParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<Guid> UpdateAsync(UpdateApisParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<Guid> ids);
    #endregion
    #region 扩展

    /// <summary>
    /// 获取树图
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    Task<Dictionary<int, List<ApisTreeSelectDTO>>> TreeSelectAsync();
    #endregion
}
