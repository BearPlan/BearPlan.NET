using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Pager;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.System;

[Route("/api/[controller]/[action]")]
public class TenantController(ITenantService service) : BaseApiController
{
    private readonly ITenantService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<TenantDTO>> GetPageAsync([FromQuery] TenantParam param) =>
        await _service.GetPageAsync(param);

    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<TenantInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateTenantParam param) => await _service.AddAsync(param);

    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateTenantParam param) => await _service.UpdateAsync(param);

    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids) => await _service.DeleteAsync(ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 获取所有租户
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<List<TenantDTO>> QueryAllAsync() => await _service.QueryAllAsync();
    #endregion
}
