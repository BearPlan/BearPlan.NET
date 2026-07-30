using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 岗位接口
/// </summary>
public interface IJobService : IBaseServices<Job>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<JobDTO>> GetPageAsync(JobParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<JobInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateJobParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateJobParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 查询所有已启用的岗位
    /// </summary>
    Task<List<JobInfo>> QueryAllAsync();
    #endregion
}
