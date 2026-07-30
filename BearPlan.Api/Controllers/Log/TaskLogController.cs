using System;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Pager;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Log;

[Route("/api/[controller]/[action]")]
public class TaskLogController(IQuartzNetLogService service) : BaseApiController
{
    private readonly IQuartzNetLogService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<QuartzNetLogDTO>> GetPageAsync([FromQuery] QuartzNetLogParam param) =>
        await _service.GetPageAsync(param);
    #endregion
}
