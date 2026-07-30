using BearPlan.Entity.Core.Permission.Role;
using BearPlan.Models.Auth;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 权限信息接口
/// </summary>
public interface IPermissionService : IBaseServices<Role>
{
    /// <summary>
    /// 获取权限标识符
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    Task<List<string>> GetAuthCodeAsync(long userId);


    /// <summary>
    /// 获取权限urls
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<List<UrlAccessControlVo>> GetUrlAccessControlAsync(long userId);
}
