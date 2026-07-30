using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Model;
using BearPlan.Entity.Core.Permission;
using BearPlan.Entity.Core.Permission.Role;
using BearPlan.Entity.Core.Permission.User;
using BearPlan.Models.Auth;
using BearPlan.Common.Global;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 权限服务
/// </summary>
public class PermissionService : BaseServices<Role>, IPermissionService
{
    #region 基础方法

    /// <summary>
    /// 获取权限标识符
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    [UseCache(Expiration = 60, KeyPrefix = GlobalConstants.CachePrefix.UserAuthCodes)]
    public async Task<List<string>> GetAuthCodeAsync(long userId)
    {
      

        var authCodeList = await SugarClient
            .Queryable<UserRole, RoleMenu, Menu>((ur, rm, m) => ur.RoleId == rm.RoleId && rm.MenuId == m.Id)
            .GroupBy((ur, rm, m) => m.Permission)
            .Where((ur, rm, m) => ur.UserId == userId && m.MenuType != MenuTypeEnum.Directory && m.Permission != null)
            .OrderBy((ur, rm, m) => m.Permission)
            .ClearFilter<ICreateByEntity>()
            .Select((ur, rm, m) => m.Permission).ToListAsync();
        authCodeList = authCodeList.Where(x => !x.IsNullOrEmpty()).ToList();
        return authCodeList;
    }


    /// <summary>
    /// 获取权限urls
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [UseCache(Expiration = 60, KeyPrefix = GlobalConstants.CachePrefix.UserAuthUrls)]
    public async Task<List<UrlAccessControlVo>> GetUrlAccessControlAsync(long userId)
    {
        var urlAccessControlList = await SugarClient
            .Queryable<UserRole, RoleApis, Apis>((ur, ra, a) => ur.RoleId == ra.RoleId && ra.ApisId == a.Id)
            .GroupBy((ur, ra, a) => new { a.Url, a.Method })
            .Where(ur => ur.UserId == userId)
            .OrderBy((ur, ra, a) => a.Url)
            .ClearFilter<ICreateByEntity>()
            .Select((ur, ra, a) => new UrlAccessControlVo
            {
                Url = a.Url,
                Method = a.Method
            }).ToListAsync();
        urlAccessControlList = urlAccessControlList.Where(x => !x.IsNullOrEmpty()).ToList();
        return urlAccessControlList;
    }

    #endregion
}
