using System.Linq.Expressions;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Model;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Core.Utils;
using BearPlan.Entity.Core.Permission;
using BearPlan.Entity.Core.Permission.Role;
using BearPlan.Entity.Core.Permission.User;
using SqlSugar;
using BearPlan.Common.Global;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 菜单服务
/// </summary>
public class MenuService : BaseServices<Menu>, IMenuService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<MenuDTO>> GetPageAsync(MenuParam param)
    {
        var page = await GetIQueryable().Select(x => new MenuDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<MenuInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select(x => new MenuInfo
        {

            //Title = x.Title,
            //Path = null,
            //Component = null,
            //ComponentName = null,
            //ParentId = null,
            //Sort = 0,
            //Icon = null,
            //Type = (MenuTypeEnum)0,
            //KeepAlive = false,
            //Hidden = false,
            //Redirect = null,
            //AlwaysShow = false,
            //Status = false,
            //IsDeleted = false,
            //Roles = null,
            //Children = null

        }, true).FirstAsync();



        entity.Querys = await GetIQueryable(x => x.ParentId == id && x.MenuType == MenuTypeEnum.Query).Select(x => new MenuQuery
        {
            Id = x.Id,
            Key = x.Name,
            Value = x.Title,
            ParentId = x.ParentId,
            Status = x.Status
        }).ToListAsync();

        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    [UseTran]
    public async Task<long> AddAsync(UpdateMenuParam param)
    {
        // 标题唯一验证
        if (await GetIQueryable(m => m.Title == param.Title).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Title)));
        }

        // 权限标识符验证（目录/菜单类型需要Permission）
        if (param.MenuType != MenuTypeEnum.Directory && param.MenuType != MenuTypeEnum.Menu)
        {
            if (param.Permission.IsNullOrEmpty())
            {
                throw new BusException(ValidationError.Required(param, nameof(param.Permission)));
            }
        }

        // 权限标识符唯一验证
        if (param.MenuType != MenuTypeEnum.Directory && param.MenuType != MenuTypeEnum.Menu &&
            await GetIQueryable(x => x.Permission == param.Permission).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Permission)));
        }

        // 组件名称唯一验证
        if (!param.Name.IsNullOrEmpty() &&
            await GetIQueryable(m => m.Name == param.Name).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Name)));
        }

        var model = App.Mapper.MapTo<Menu>(param);


        await AddAsync(model);




        var addQuerys = param.Querys?.Select(x => new Menu
        {
            MenuType = MenuTypeEnum.Query,
            Title = x.Key,
            Name = x.Value,
            ParentId = model.Id,
            Status = x.Status
        }).ToList();
        await AddAsync(addQuerys);

        var menu = App.Mapper.MapTo<Menu>(param);
        await AddAsync(menu);

        // 清除缓存
        if (menu.ParentId > 0)
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadMenusByPId +
                                        menu.ParentId.ToString().ToMd5String16());
        }

        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadAllMenu);
        return menu.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    [UseTran]
    public async Task<long> UpdateAsync(UpdateMenuParam param)
    {
        var oldMenu = await GetIQueryable(x => x.Id == param.Id).FirstAsync();
        if (oldMenu == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        // 标题唯一验证
        if (oldMenu.Title != param.Title &&
            await GetIQueryable(x => x.Title == param.Title).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Title)));
        }

        // 权限标识符验证
        if (param.MenuType != MenuTypeEnum.Directory && param.MenuType != MenuTypeEnum.Menu)
        {
            if (param.Permission.IsNullOrEmpty())
            {
                throw new BusException(ValidationError.Required(param, nameof(param.Permission)));
            }
        }

        // 权限标识符唯一验证
        if (param.MenuType != MenuTypeEnum.Directory && param.MenuType != MenuTypeEnum.Menu &&
            await GetIQueryable(x => x.Permission == param.Permission).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Permission)));
        }

        // 组件名称唯一验证
        if (!param.Name.IsNullOrEmpty())
        {
            if (oldMenu.Name != param.Name &&
                await GetIQueryable(m => m.Name == param.Name).AnyAsync())
            {
                throw new BusException(ValidationError.IsExist(param, nameof(param.Name)));
            }
        }

        var model = App.Mapper.MapTo<Menu>(param);
        await UpdateAsync(model);




        //新增/编辑
        var querys = param.Querys.Select(x => new Menu
        {
            Id = x.Id,
            MenuType = MenuTypeEnum.Query,
            Title = x.Key,
            Name = x.Value,
            ParentId = param.Id,
            Status = x.Status
        }).ToList();

        //删除
        await LogicDeleteAsync<Menu>(x => x.MenuType == MenuTypeEnum.Query && x.ParentId == param.Id && querys.Any(z => z.Id != x.Id));
        await SugarClient.Storageable(querys).ExecuteCommandAsync();

        // 清除缓存
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadMenusById +
                                    model.Id.ToString().ToMd5String16());
        if (model.ParentId > 0)
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadMenusByPId +
                                        model.ParentId.ToString().ToMd5String16());
        }

        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadAllMenu);
        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    [UseTran]
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        var menuList = await GetIQueryable(x => ids.Contains(x.Id)).ToListAsync();
        if (menuList.Count < 1)
        {
            throw new BusException(ValidationError.NotExist());
        }

        // 递归查找所有子菜单ID
        var idList = new List<long>();
        foreach (var id in ids)
        {
            if (!idList.Contains(id))
            {
                idList.Add(id);
            }

            var menus = await GetIQueryable(m => m.ParentId == id).ToListAsync();
            await FindChildIdsAsync(menus, idList);
        }

        var result = await DeleteAsync(x => idList.Contains(x.Id));

        // 清除缓存
        if (result > 0)
        {
            foreach (var id in idList)
            {
                await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadMenusById +
                                            id.ToString().ToMd5String16());
                await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadMenusByPId +
                                            id.ToString().ToMd5String16());
            }
        }

        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadAllMenu);
        return result;
    }
    #endregion

    #region 扩展
    /// <summary>
    /// 常量路由
    /// </summary>
    /// <returns></returns>
    //[UseCache(Expiration = 120, KeyPrefix = GlobalConstants.CachePrefix.UserMenuConstant)]
    public async Task<List<RouteDTO>> ConstantRoutesAsync()
    {
        var entity = await GetIQueryable(x => x.Constant).ToListAsync();
        return await BuildAsync(entity);
    }


    /// <summary>
    /// 构建前端路由菜单
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    //[UseCache(Expiration = 120, KeyPrefix = GlobalConstants.CachePrefix.UserMenuById)]
    public async Task<List<RouteDTO>> BuildTreeAsync(long userId)
    {
        var menuList = await SugarClient
            .Queryable<UserRole, RoleMenu, Menu>((ur, rm, m) => ur.RoleId == rm.RoleId && rm.MenuId == m.Id)
            .Where((ur, rm, m) => ur.UserId == userId && m.MenuType != MenuTypeEnum.Query)
            .OrderBy((ur, rm, m) => m.Order)
            .ClearFilter<ICreateByEntity>()
            .Select((ur, rm, m) => m).Distinct().ToListAsync();
        //var menuListChild = TreeHelper<Menu>.ListToTrees(menuList, "Id", "ParentId", 0);
        return await BuildAsync(menuList);
    }



    /// <summary>
    /// 判断路由是否存在
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsRouteExistAsync(string name)
    {
        Expression<Func<Menu, bool>> where = x => x.Status;
        where = where.And(x => x.Name == name);
        return await GetIQueryable(where).AnyAsync();
    }


    /// <summary>
    /// 列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<MenuTreeDTO>> GetTreeAsync()
    {
        //排除公共模块

        var entity = await GetIQueryable(x => !x.Constant).OrderBy(x => x.Order).Select<MenuTreeDTO>(x => new MenuTreeDTO
        {

            Path = "/" + x.Name + x.PathParam,

        }, true).ToListAsync();

        var list = new List<MenuTreeDTO>();

        entity.ForEach(x =>
        {

            x.Children = entity.Where(y => y.ParentId == x.Id).Select(y =>
            {
                if (y.MenuType != MenuTypeEnum.Query)
                {
                    y.Path = x.Path + y.Path;
                    y.Name = x.Name + "_" + y.Name;
                }
                return y;
            }).ToList();
            if (x.ParentId == null)
            {
                list.Add(x);
            }
        });
        return list;
    }


    /// <summary>
    /// 菜单下拉
    /// </summary>
    /// <returns></returns>
    public async Task<List<MenuTreeSelectDTO>> TreeSelectAsync(MenuTypeEnum[] types)
    {
        var tree = await GetIQueryable(x => !x.Constant)
            .WhereIF(types != null, x => types.Contains(x.MenuType))
            .OrderBy(x => x.Order)
            .Select<MenuTreeSelectDTO>().ToTreeAsync(it => it.Children, it => it.ParentId, null, it => it.Id);
        return tree;
    }


    #endregion

    #region 私有方法
    /// <summary>
    /// 构建前端路由菜单
    /// </summary>
    /// <param name="menuDtOs"></param>
    /// <returns></returns>
    private async Task<List<RouteDTO>> BuildAsync(List<Menu> menus)

    {
        //查出请求参数有
        var querys = GetIQueryable(x => x.Status && x.MenuType == MenuTypeEnum.Query && menus.Any(y => y.Id == x.ParentId)).Select(x => new MenuQuery
        {
            ParentId = x.ParentId,
            Key = x.Name,
            Status = x.Status,
            Value = x.Title
        }).ToList();
        var entity = menus.Select(x => new RouteDTO()
        {
            Id = x.Id,
            Meta = new RouteMeta
            {
                Icon = x.Icon,
                LocalIcon = x.LocalIcon,
                IconFontSize = x.IconFontSize,
                Order = x.Order,
                //Href = x.Href,
                HideInMenu = x.HideInMenu,
                ActiveMenu = x.ActiveMenu,
                MultiTab = x.MultiTab,
                FixedIndexInTab = x.FixedIndexInTab,
                //Query = x.Query,
                KeepAlive = x.KeepAlive,
                Constant = x.Constant,
                Title = x.Title,
                Query = querys.Where(y => y.ParentId == x.Id && !string.IsNullOrEmpty(y.Value) && !string.IsNullOrEmpty(y.Key)).ToList()
            },
            Props = new Dictionary<string, string> {
                { "url", x.Href }
                },

            Name = x.Name,
            Component = Component(x.Layout, x.Component),
            Path = "/" + x.Name + x.PathParam,
            ParentId = x.ParentId,
            //Type = x.Type,
            Redirect = x.Redirect,
            //Status = x.Status,

        }).ToList();


        List<RouteDTO> list = new List<RouteDTO>();

        entity.ForEach(x =>
        {
            x.Children = entity.Where(y => y.ParentId == x.Id).Select(y =>
            {
                y.Path = x.Path + y.Path;
                y.Name = x.Name + "_" + y.Name;
                return y;
            }).ToList();
            if (x.ParentId == null)
            {
                list.Add(x);
            }
        });
        return list;

        //返回组件
        string Component(LayoutTypeEnum? layout, string component)
        {
            var str = string.Empty;
            if (layout != null && !string.IsNullOrEmpty(component))
            {
                str = layout.GetDisplayName() + "$" + component;
            }

            else
                str = layout != null ? layout.GetDisplayName() : component;
            return str;
        }
    }
    /// <summary>
    /// 获取所有下级菜单
    /// </summary>
    /// <param name="menuList"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    private async Task FindChildIdsAsync(List<Menu> menuList, List<long> ids)
    {
        if (menuList is { Count: > 0 })
        {
            foreach (var menu in menuList)
            {
                if (!ids.Contains(menu.Id))
                {
                    ids.Add(menu.Id);
                }

                List<Menu> menus = await GetIQueryable(m => m.ParentId == menu.Id).ToListAsync();
                if (menus is { Count: > 0 })
                {
                    await FindChildIdsAsync(menus, ids);
                }
            }
        }

        await Task.FromResult(ids);
    }
    #endregion
}
