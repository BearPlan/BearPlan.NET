using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Helper;
using BearPlan.Core.Pager;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.Permission;

/// <summary>
/// 部门管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class DeptController(IDeptService service) : BaseApiController
{
    private readonly IDeptService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<DeptDTO>> GetPageAsync([FromQuery] DeptParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 查询详情
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<DeptInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateDeptParam param) =>
        await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateDeptParam param) =>
        await _service.UpdateAsync(param);

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids) =>
        await _service.DeleteAsync(ids);
    #endregion

    #region 扩展
    /// <summary>
    /// 获取树
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<List<DeptTreeDTO>> GetTreeAsync([FromQuery] DeptTreeParam param) => await _service.GetTreeAsync(param);
    #endregion
}
