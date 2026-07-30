using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Common.WebApp;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Pager;
using BearPlan.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.Permission;

/// <summary>
/// 菜单管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class MenuController(IMenuService service) : BaseApiController
{
    private readonly IMenuService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<MenuDTO>> GetPageAsync([FromQuery] MenuParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 查询详情
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<MenuInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateMenuParam param) =>
        await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateMenuParam param) =>
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
    /// 常量路由
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task<List<RouteDTO>> ConstantRoutesAsync() => await _service.ConstantRoutesAsync();


    /// <summary>
    /// 判断路由是否存在
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<bool> IsRouteExistAsync(string name) => await _service.IsRouteExistAsync(name);

    /// <summary>
    /// 列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<List<MenuTreeDTO>> GetTreeAsync() => await _service.GetTreeAsync();


    /// <summary>
    /// 菜单下拉
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    public async Task<List<MenuTreeSelectDTO>> TreeSelectAsync([FromBody] MenuTypeEnum[] types) => await _service.TreeSelectAsync(types);
    

    /// <summary>
    /// 我的菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    public async Task<List<RouteDTO>> MyRoutesAsync() => await _service.BuildTreeAsync(App.GetService<IHttpUser>().Id);
#endregion
}
