using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BearPlan.Common.Enums;
using BearPlan.Common.MultiLanguage.Resources;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Core.Utils;
using BearPlan.Entity.Core.Permission;
using BearPlan.Entity.Core.Permission.Role;
using BearPlan.Entity.Core.Permission.User;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;
using BearPlan.Common.Global;
using BearPlan.Common.WebApp;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 角色服务
/// </summary>
public class RoleService : BaseServices<Role>, IRoleService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<RoleDTO>> GetPageAsync(RoleParam param)
    {
        var page = await GetIQueryable().Select(x => new RoleDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<RoleInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id)
            .Includes(x => x.Menus)
            .Includes(x => x.Apis)
            .Includes(x => x.Depts)
            .Select<RoleInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    [UseTran]
    public async Task<long> AddAsync(UpdateRoleParam param)
    {
        await VerificationUserRoleLevelAsync(param.Level);

        if (await GetIQueryable(r => r.Name == param.Name).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Name)));
        }

        if (await GetIQueryable(r => r.AuthCode == param.AuthCode).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.AuthCode)));
        }

        if (param.DataScopeType == DataScopeType.Customize &&
            (param.DeptIds == null || param.DeptIds.Count == 0))
        {
            throw new BusException(string.Format(Language.Arg0_AtLeastOne, Language.Sys_Dept));
        }

        var role = App.Mapper.MapTo<Role>(param);
        await AddAsync(role);

        if (param.DataScopeType == DataScopeType.Customize && param.DeptIds != null)
        {
            var roleDepts = param.DeptIds.Select(d => new RoleDept
            { RoleId = role.Id, DeptId = d }).ToList();
            await SugarClient.Insertable(roleDepts).ExecuteCommandAsync();
        }

        return role.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    [UseTran]
    public async Task<long> UpdateAsync(UpdateRoleParam param)
    {
        var oldRole = await GetIQueryable(x => x.Id == param.Id)
            .Includes(x => x.Users).FirstAsync();
        if (oldRole == null)
        {
            throw new BusException(ValidationError.NotExist());
        }

        if (oldRole.Name != param.Name &&
            await GetIQueryable(x => x.Name == param.Name).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.Name)));
        }

        if (oldRole.AuthCode != param.AuthCode &&
            await GetIQueryable(x => x.AuthCode == param.AuthCode).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param, nameof(param.AuthCode)));
        }

        if (param.DataScopeType == DataScopeType.Customize &&
            (param.DeptIds == null || param.DeptIds.Count == 0))
        {
            throw new BusException(string.Format(Language.Arg0_AtLeastOne, Language.Sys_Dept));
        }

        await VerificationUserRoleLevelAsync(param.Level);
        var role = App.Mapper.MapTo<Role>(param);
        await UpdateAsync(role);

        // 删除并重建部门权限关联
        await SugarClient.Deleteable<RoleDept>().Where(x => x.RoleId == role.Id).ExecuteCommandAsync();
        if (param.DataScopeType == DataScopeType.Customize && param.DeptIds != null)
        {
            var roleDepts = param.DeptIds.Select(d => new RoleDept
            { RoleId = role.Id, DeptId = d }).ToList();
            await SugarClient.Insertable(roleDepts).ExecuteCommandAsync();
        }

        // 清除用户缓存
        if (oldRole.Users != null)
        {
            foreach (var user in oldRole.Users)
            {
                await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserDataScopeById +
                                            user.Id.ToString().ToMd5String16());
            }
        }

        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    [UseTran]
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        var roles = await GetIQueryable(x => ids.Contains(x.Id))
            .Includes(x => x.Users).ToListAsync();
        if (roles.Count == 0)
        {
            throw new BusException(ValidationError.NotExist());
        }

        if (roles.Any(role => role.Users != null && role.Users.Count != 0))
        {
            throw new BusException(ValidationError.DataAssociationExists());
        }

        var minLevel = roles.Select(x => x.Level).Min();
        await VerificationUserRoleLevelAsync(minLevel);

        // 删除角色及关联表
        await SugarClient.Deleteable<RoleDept>().Where(x => ids.Contains(x.RoleId)).ExecuteCommandAsync();
        await SugarClient.Deleteable<RoleMenu>().Where(x => ids.Contains(x.RoleId)).ExecuteCommandAsync();
        await SugarClient.Deleteable<RoleApis>().Where(x => ids.Contains(x.RoleId)).ExecuteCommandAsync();
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion

    #region 扩展
    /// <summary>
    /// 查询全部
    /// </summary>
    public async Task<List<RoleInfo>> QueryAllAsync()
    {
        return await GetIQueryable()
            .Includes(x => x.Menus)
            .Includes(x => x.Depts)
            .Select<RoleInfo>().ToListAsync();
    }

    /// <summary>
    /// 查询角色等级
    /// </summary>
    public async Task<int?> QueryUserRoleLevelAsync(HashSet<long> ids)
    {
        var levels = await SugarClient.Queryable<Role, UserRole>((r, ur) => new JoinQueryInfos(
                JoinType.Left, r.Id == ur.RoleId
            )).Where((r, ur) => ids.Contains(ur.UserId))
            .Select((r) => r.Level).ToListAsync();
        if (levels.Any())
        {
            return levels.Min();
        }

        return null;
    }

    /// <summary>
    /// 验证用户角色等级
    /// </summary>
    public async Task<int> VerificationUserRoleLevelAsync(int? level)
    {
        var minLevel = 999;
        var levels = await SugarClient.Queryable<Role, UserRole>((r, ur) => new JoinQueryInfos(
                JoinType.Left, r.Id == ur.RoleId
            )).Where((r, ur) => ur.UserId == App.GetService<IHttpUser>().Id)
            .Select((r) => r.Level).ToListAsync();

        if (levels.Any())
        {
            minLevel = levels.Min();
        }

        if (level != null && level < minLevel)
        {
            throw new BadRequestException(Language.Error_PermissionDenied_HigherRoleData);
        }

        return minLevel;
    }

    ///// <summary>
    ///// 更新角色菜单
    ///// </summary>
    //[UseTran]
    //public async Task UpdateRoleMenuAsync(UpdateRoleMenuParam param)
    //{
    //    var role = await GetIQueryable(x => x.Id == param.Id)
    //        .Includes(x => x.Users).FirstAsync();
    //    if (role == null)
    //    {
    //        throw new BusException(ValidationError.NotExist());
    //    }

    //    await VerificationUserRoleLevelAsync(role.Level);

    //    var roleMenus = param.MenuIds.Select(m => new RoleMenu
    //    { RoleId = role.Id, MenuId = m }).ToList();

    //    await SugarClient.Deleteable<RoleMenu>().Where(x => x.RoleId == role.Id).ExecuteCommandAsync();
    //    await SugarClient.Insertable(roleMenus).ExecuteCommandAsync();

    //    // 清除用户缓存
    //    if (role.Users != null)
    //    {
    //        foreach (var user in role.Users)
    //        {
    //            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserAuthCodes +
    //                                        user.Id.ToString().ToMd5String16());
    //            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserMenuById +
    //                                        user.Id.ToString().ToMd5String16());
    //        }
    //    }
    //}

    ///// <summary>
    ///// 更新角色Api路由
    ///// </summary>
    //[UseTran]
    //public async Task UpdateRoleApiAsync(UpdateRoleApiParam param)
    //{
    //    var role = await GetIQueryable(x => x.Id == param.Id)
    //        .Includes(x => x.Users).FirstAsync();
    //    if (role == null)
    //    {
    //        throw new BusException(ValidationError.NotExist());
    //    }

    //    await VerificationUserRoleLevelAsync(role.Level);

    //    // 过滤自生成的一级节点ID
    //    var apiIds = param.ApiIds.Where(x => x > 10000).ToList();
    //    var roleApis = apiIds.Select(a => new RoleApis
    //    { RoleId = role.Id, ApisId = a }).ToList();

    //    await SugarClient.Deleteable<RoleApis>().Where(x => x.RoleId == role.Id).ExecuteCommandAsync();
    //    await SugarClient.Insertable(roleApis).ExecuteCommandAsync();

    //    // 清除用户缓存
    //    if (role.Users != null)
    //    {
    //        foreach (var user in role.Users)
    //        {
    //            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserAuthUrls +
    //                                        user.Id.ToString().ToMd5String16());
    //            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserMenuById +
    //                                        user.Id.ToString().ToMd5String16());
    //        }
    //    }
    //}


    /// <summary>
    /// 获取权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<RoleApisParam> GetApisAsync(long roleId)
    {
        var model = new RoleApisParam() { RoleId = roleId };
        var role = await GetIQueryable()
    .Includes(x => x.Menus)       // Load Menus navigation property
    .Includes(x => x.Apis)        // Load Apis navigation property
    .Where(x => x.Id == roleId)
    .Select(x => new {
        MenuIds = x.Menus.Select(m => m.Id).ToList(),
        Apis = x.Apis.Select(a => new { a.Id, a.Version }).ToList()
    })
    .FirstAsync();
        foreach (var item in Enum.GetValues(typeof(VersionEnum)))
        {

            VersionEnum version = (VersionEnum)item;
            var ids = role.Apis.Where(x => x.Version == version).Select(x => x.Id).ToList();
            model.ApiIds.Add((int)version, ids);
        }
        model.MenuIds = role.MenuIds;
        return model;
    }

    /// <summary>
    /// 设置权限
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task SetApisAsync(RoleApisParam param)
    {
        var entity = new Role
        {
            Id = param.RoleId

        };
        entity.Menus = param.MenuIds.Select(x => new Menu { Id = x }).ToList();
        entity.Apis = new List<Apis>();
        foreach (var item in param.ApiIds)
        {
            entity.Apis.AddRange(item.Value.Select(y => new Apis
            {
                Id = y,
                Version = (VersionEnum)item.Key
            }).ToList());
        }



        SugarRepository.SugarClient.UpdateNav(entity)
            .Include(z1 => z1.Menus)
             .Include(z1 => z1.Apis)
            .ExecuteCommand();
    }
    #endregion
}
