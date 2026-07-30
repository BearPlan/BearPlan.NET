using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Exception;
using BearPlan.Core.Pager;
using BearPlan.Common.WebApp;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.IBusiness;
using BearPlan.Models.Core.Monitor;
using BearPlan.Models.Queries.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.Permission;

/// <summary>
/// 在线用户
/// </summary>
[Route("/api/[controller]/[action]")]
public class OnlineUserController(IOnlineUserService service) : BaseApiController
{
    #region 字段

    private readonly IOnlineUserService _service = service ?? throw new ArgumentNullException(nameof(service));
    #endregion

    #region 对内接口

    /// <summary>
    /// 在线用户列表
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<OnlineUserDTO>> GetPageAsync([FromQuery] OnlineUserParam param) => await _service.GetPageAsync(param);

    /// <summary>
    /// 强制登出用户
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task DropOutAsync([FromBody] IdCollectionString idCollection)
    {
        await _service.DropOutAsync(idCollection.IdArray);
    }
    #endregion
}
