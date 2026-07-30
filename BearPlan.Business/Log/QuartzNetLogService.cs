using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Log;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Log;

/// <summary>
/// QuartzNet作业日志服务
/// </summary>
public class QuartzNetLogService : BaseServices<QuartzNetLog>, IQuartzNetLogService
{
    #region CRUD
    public async Task<PagedResults<QuartzNetLogDTO>> GetPageAsync(QuartzNetLogParam param)
    {
        var page = await GetIQueryable(isSplitTable: true)
            .Select(x => new QuartzNetLogDTO
            {
                Id = x.Id,
            }, true)
            .SearchWhere(param)
            .ToPagedResultsAsync(param);
        return page;
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 创建任务日志
    /// </summary>
    public async Task CreateAsync(QuartzNetLog quartzNetLog)
    {
        await SugarRepository.SugarClient.Insertable(quartzNetLog).SplitTable().ExecuteCommandAsync();
    }
    #endregion
}
