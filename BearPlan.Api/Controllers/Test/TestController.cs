using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.IBusiness;
using BearPlan.Models.Test;
using BearPlan.Core.Pager;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Test;

/// <summary>
/// 测试订单
/// </summary>
[Route("/api/[controller]/[action]")]
public class TestController(ITestOrderService service) : BaseApiController
{
    private readonly ITestOrderService _service = service ?? throw new ArgumentNullException(nameof(service));
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<TestOrderDTO>> GetPageAsync([FromQuery] TestOrderParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 查询详情
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<TestOrderInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateTestOrderParam param) => await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateTestOrderParam param) => await _service.UpdateAsync(param);

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids) => await _service.DeleteAsync(ids);
    #endregion
}
