using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models.Core.Permission;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 菜单接口
/// </summary>
public interface IMenuService : IBaseServices<Menu>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<MenuDTO>> GetPageAsync(MenuParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<MenuInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateMenuParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateMenuParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion

    #region 扩展

    /// <summary>
    /// 构建前端菜单树
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<List<RouteDTO>> BuildTreeAsync(long userId);


    /// <summary>
    /// 常量路由
    /// </summary>
    /// <returns></returns>
    Task<List<RouteDTO>> ConstantRoutesAsync();

    /// <summary>
    /// 判断路由是否存在
    /// </summary>
    /// <returns></returns>
    Task<bool> IsRouteExistAsync(string name);

    /// <summary>
    /// 列表
    /// </summary>
    /// <returns></returns>
    Task<List<MenuTreeDTO>> GetTreeAsync();
    /// <summary>
    /// 菜单下拉
    /// </summary>
    /// <returns></returns>
    Task<List<MenuTreeSelectDTO>> TreeSelectAsync(MenuTypeEnum[] types);
    #endregion
}
