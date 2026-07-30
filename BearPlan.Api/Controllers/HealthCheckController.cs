using System.Threading.Tasks;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers;

/// <summary>
/// 健康检测
/// </summary>
[Route("/api/[controller]/[action]")]
public class HealthCheckController : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    [NotAudit]
    public async Task StatusAsync()
    {
        return;
    }
}
