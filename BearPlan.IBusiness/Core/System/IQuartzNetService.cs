using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// QuartzJob作业接口
/// </summary>
public interface IQuartzNetService : IBaseServices<QuartzNet>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<QuartzNetDTO>> GetPageAsync(QuartzNetParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<QuartzNetInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateQuartzNetParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateQuartzNetParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 更新任务与日志
    /// </summary>
    Task UpdateJobInfoAsync(QuartzNet quartzNet);

    /// <summary>
    /// 查询全部
    /// </summary>
    Task<List<QuartzNet>> QueryAllAsync();

    /// <summary>
    /// 查询全部作业名称
    /// </summary>
    Task<List<QuartzNetDTO>> QueryAllTaskNameAsync();
    #endregion
}
