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
using BearPlan.Common.WebApp;

namespace BearPlan.Business.Log;

/// <summary>
/// 操作日志服务
/// </summary>
public class AuditLogService : BaseServices<AuditLog>, IAuditLogService
{
    #region CRUD
    public async Task<PagedResults<AuditLogDTO>> GetPageAsync(AuditLogParam param)
    {
        var page = await GetIQueryable(isSplitTable: true)
            .Select(x => new AuditLogDTO
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
    /// 创建操作日志
    /// </summary>
    public async Task CreateAsync(AuditLog operateLog)
    {
        await SugarRepository.SugarClient.Insertable(operateLog).SplitTable().ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量创建操作日志
    /// </summary>
    public async Task CreateListAsync(List<AuditLog> operateLogs)
    {
        await SugarRepository.SugarClient.Insertable(operateLogs).SplitTable().ExecuteCommandAsync();
    }

    /// <summary>
    /// 查询当前用户操作日志
    /// </summary>
    public async Task<PagedResults<AuditLogDTO>> QueryByCurrentAsync(AuditLogParam param)
    {
        var page = await GetIQueryable(x => x.CreateBy == App.GetService<IHttpUser>().Account, isSplitTable: true)
            .Select(x => new AuditLogDTO
            {
                Id = x.Id,
            }, true)
            .SearchWhere(param)
            .ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 获取操作日志数量
    /// </summary>
    public async Task<List<int>> GetOperationNumber(int days = 7)
    {
        DateTime startDate = DateTime.Now.AddDays(-(days - 1));

        var dateList = Enumerable.Range(0, days)
            .Select(i => startDate.AddDays(i).ToString("yyyy-MM-dd"))
            .ToList();

        var list = await GetIQueryable().Where(x => x.CreateTime >= startDate)
            .GroupBy(x => x.CreateTime.ToString("yyyy-MM-dd"))
            .Select(it => new
            {
                Time = it.CreateTime.ToString("yyyy-MM-dd"),
                Count = SqlFunc.AggregateCount(it.Id)
            })
            .OrderBy(it => it.Time)
            .ToListAsync();

        var dict = list.ToDictionary(x => x.Time, x => x.Count);
        var numbers = dateList.Select(date => dict.ContainsKey(date) ? dict[date] : 0).ToList();
        return numbers;
    }
    #endregion
}
