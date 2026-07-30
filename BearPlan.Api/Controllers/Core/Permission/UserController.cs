using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.Permission;

/// <summary>
/// 用户管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class UserController(IUserService service) : BaseApiController
{
    private readonly IUserService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<UserDTO>> GetPageAsync([FromQuery] UserParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 查询详情
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<UserInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateUserParam param) =>
        await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateUserParam param) =>
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
    /// 修改个人中心信息
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task UpdateCenterAsync(UpdateUserCenterParam param) =>
        await _service.UpdateCenterAsync(param);

    /// <summary>
    /// 修改密码
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task UpdatePasswordAsync(UpdateUserPassParam param) =>
        await _service.UpdatePasswordAsync(param);

    /// <summary>
    /// 修改偏好配置
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task UpdatePreferencesConfigAsync(UpdateUserPreferencesConfigParam param) =>
        await _service.UpdatePreferencesConfigAsync(param);

    /// <summary>
    /// 修改邮箱
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task UpdateEmailAsync(UpdateUserEmailParam param) =>
        await _service.UpdateEmailAsync(param);

    /// <summary>
    /// 修改头像
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<string> UpdateAvatarAsync(IFormFile avatar) =>
        await _service.UpdateAvatarAsync(avatar);

    /// <summary>
    /// 修改用户角色
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task UpdateUserRoleAsync(UpdateUserRoleParam param) =>
        await _service.UpdateUserRoleAsync(param);

    /// <summary>
    /// 修改用户岗位
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task UpdateUserJobAsync(UpdateUserJobParam param) =>
        await _service.UpdateUserJobAsync(param);
    #endregion
}
