using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.Permission;

/// <summary>
/// 角色管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class RoleController(IRoleService service) : BaseApiController
{
    private readonly IRoleService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<RoleDTO>> GetPageAsync([FromQuery] RoleParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 查询详情
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<RoleInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateRoleParam param) =>
        await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateRoleParam param) =>
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
    /// 获取全部角色
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<List<RoleInfo>> QueryAllAsync() =>
        await _service.QueryAllAsync();

    /// <summary>
    /// 获取当前用户角色等级
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> GetRoleLevelAsync(int? level) =>
        await _service.VerificationUserRoleLevelAsync(level);

    ///// <summary>
    ///// 更新角色菜单关联
    ///// </summary>
    //[HttpPut]
    //[ApiVersion("1.0", Deprecated = false)]
    //public async Task UpdateRoleMenuAsync(RoleApisParam param) =>
    //    await _service.UpdateRoleMenuAsync(param);

    ///// <summary>
    ///// 更新角色Api关联
    ///// </summary>
    //[HttpPut]
    //[ApiVersion("1.0", Deprecated = false)]
    //public async Task UpdateRoleApiAsync(RoleApisParam param) =>
    //    await _service.UpdateRoleApiAsync(param);


    /// <summary>
    /// 获取权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task<RoleApisParam> GetApisAsync(long roleId) => await _service.GetApisAsync(roleId);
    /// <summary>
    /// 设置权限
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task SetApisAsync([FromBody] RoleApisParam param) => await _service.SetApisAsync(param);
    #endregion
}
