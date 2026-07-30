using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System.Dict;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// 字典详情接口
/// </summary>
public interface IDictDetailService : IBaseServices<DictDetail>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<DictDetailDTO>> GetPageAsync(DictDetailParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<DictDetailInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateDictDetailParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateDictDetailParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 根据字典ID查询详情
    /// </summary>
    Task<List<DictDetailInfo>> GetDetailByDictIdAsync(long dictId);
    #endregion
}
