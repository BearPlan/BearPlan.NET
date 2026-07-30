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

/// <summary>
/// 字典管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class DictController(IDictService service) : BaseApiController
{
    private readonly IDictService _service = service ?? throw new ArgumentNullException(nameof(service));
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<DictDTO>> GetPageAsync([FromQuery] DictParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 查询详情
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<DictInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateDictParam param) => await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateDictParam param) => await _service.UpdateAsync(param);

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids) => await _service.DeleteAsync(ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 根据名称查询字典
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<DictInfo> QueryByNameAsync(string name) => await _service.QueryByNameAsync(name);
    #endregion
}
