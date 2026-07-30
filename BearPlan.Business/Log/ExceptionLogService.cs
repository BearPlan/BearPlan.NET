using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Log;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Log;

/// <summary>
/// 系统日志服务
/// </summary>
public class ExceptionLogService : BaseServices<ExceptionLog>, IExceptionLogService
{
    #region CRUD
    public async Task<PagedResults<ExceptionLogDTO>> GetPageAsync(ExceptionLogParam param)
    {
        var page = await GetIQueryable(isSplitTable: true)
            .Select(x => new ExceptionLogDTO
            {
            }, true)
            .SearchWhere(param)
            .ToPagedResultsAsync(param);
        return page;
    }

    public async Task<ExceptionLogInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id, isSplitTable: true)
            .Select<ExceptionLogInfo>().FirstAsync();
        return entity;
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 创建异常日志
    /// </summary>
    public async Task CreateAsync(ExceptionLog exceptionLog)
    {
        await SugarRepository.SugarClient.Insertable(exceptionLog).SplitTable().ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取异常日志数量
    /// </summary>
    public async Task<List<int>> GetOperationNumber(int days = 7)
    {
        DateTime startDate = DateTime.Now.AddDays(-(days - 1));

        var dateList = Enumerable.Range(0, days)
            .Select(i => startDate.AddDays(i).ToString("yyyy-MM-dd"))
            .ToList();

        var list = await GetIQueryable(x => x.CreateTime >= startDate)
            .GroupBy(x => x.CreateTime.ToString("yyyy-MM-dd"))
            .Select(it => new { Time = it.CreateTime.ToString("yyyy-MM-dd"), Count = SqlFunc.AggregateCount(it.Id) })
            .OrderBy(it => it.Time)
            .ToListAsync();

        var dict = list.ToDictionary(x => x.Time, x => x.Count);
        var numbers = dateList.Select(date => dict.ContainsKey(date) ? dict[date] : 0).ToList();
        return numbers;
    }
    #endregion
}
