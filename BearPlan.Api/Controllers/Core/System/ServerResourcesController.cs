using System.ComponentModel;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Attributes;
using BearPlan.Core.Exception;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.IBusiness;
using BearPlan.Models.ServerInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.System;

/// <summary>
/// 服务器管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class ServerResourcesController : BaseApiController
{
    private readonly IServerResourcesService _serverResourcesService;

    public ServerResourcesController(IServerResourcesService serverResourcesService)
    {
        _serverResourcesService = serverResourcesService;
    }

    #region 对内接口

    [HttpGet]
    [NotAudit]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<ServerResourcesInfo> GetInfoAsync()
    {
        if (!App.GetOptions<SystemOptions>().UseRedisCache)
        {
            throw new  BusException ("该功能需要使用Redis缓存，请配置UseServerResources为True使用。");
        }

        var resourcesInfo = await _serverResourcesService.GetInfoAsync();

        return resourcesInfo;
    }

    #endregion
}
