using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models;

namespace BearPlan.IBusiness.Log;

/// <summary>
/// QuartzJob日志接口
/// </summary>
public interface IQuartzNetLogService : IBaseServices<QuartzNetLog>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<QuartzNetLogDTO>> GetPageAsync(QuartzNetLogParam param);
    #endregion
    #region 扩展
    /// <summary>
    /// 创建任务日志（内部方法）
    /// </summary>
    Task CreateAsync(QuartzNetLog quartzNetLog);
    #endregion
}
