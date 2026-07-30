using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Exception;
using BearPlan.Core.Pager;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Log;

[Route("/api/[controller]/[action]")]
public class AuditLogController(IAuditLogService operateLogService, IExceptionLogService exceptionLogService)
    : BaseApiController
{
    private readonly IAuditLogService _operateLogService = operateLogService ?? throw new ArgumentNullException(nameof(operateLogService));
    private readonly IExceptionLogService _exceptionLogService = exceptionLogService ?? throw new ArgumentNullException(nameof(exceptionLogService));

    #region CRUD
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<AuditLogDTO>> GetPageAsync([FromQuery] AuditLogParam param) =>
        await _operateLogService.GetPageAsync(param);
    #endregion
    #region 扩展
    /// <summary>
    /// 当前用户行为
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<AuditLogDTO>> QueryByCurrentAsync([FromQuery] AuditLogParam param) =>
        await _operateLogService.QueryByCurrentAsync(param);

    /// <summary>
    /// 访问趋势
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<object> QueryVisitTrendAsync(int days = 30)
    {
        if (days < 1 || days > 30)
        {
            throw new BusException("Days must be between 1 and 30.");
        }

        var startDate = DateTime.Now.AddDays(-(days - 1));

        var dateList = Enumerable.Range(0, days)
            .Select(i => startDate.AddDays(i).ToString("yyyy-MM-dd"))
            .ToList();

        var operateNumbers = await _operateLogService.GetOperationNumber(days);
        var exceptionNumbers = await _exceptionLogService.GetOperationNumber(days);
        return new { DateList = dateList, AuditList = operateNumbers, ExceptionList = exceptionNumbers };
    }
    #endregion
}
