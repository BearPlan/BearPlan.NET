using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System.Dict;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// 字典接口
/// </summary>
public interface IDictService : IBaseServices<Dict>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<DictDTO>> GetPageAsync(DictParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<DictInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateDictParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateDictParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 根据名称查询字典
    /// </summary>
    Task<DictInfo> QueryByNameAsync(string name);
    #endregion
}
