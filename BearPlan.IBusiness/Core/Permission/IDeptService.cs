using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 部门接口
/// </summary>
public interface IDeptService : IBaseServices<Dept>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<DeptDTO>> GetPageAsync(DeptParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<DeptInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateDeptParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateDeptParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion

    #region 扩展

    /// <summary>
    /// 获取树
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>

    Task<List<DeptTreeDTO>> GetTreeAsync(DeptTreeParam param);
    #endregion
}
