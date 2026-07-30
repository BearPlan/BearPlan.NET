using BearPlan.Core.Pager;
using BearPlan.Common.WebApp;
using BearPlan.Models.Auth;
using BearPlan.Models.Core.Permission.User;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 在线用户接口
/// </summary>
public interface IOnlineUserService
{
    #region 基础接口

    /// <summary>
    /// 保存在线用户
    /// </summary>
    /// <param name="UserInfo"></param>
    /// <param name="remoteIp"></param>
    Task<LoginUserInfo> SaveLoginUserAsync(JwtUserInfo jwtUserInfo, string remoteIp);

    /// <summary>
    /// jwt用户信息
    /// </summary>
    /// <param name="UserInfo"></param>
    /// <param name="authCodeList"></param>
    /// <returns></returns>
    Task<JwtUserInfo> CreateJwtUserAsync(UserInfo UserInfo, List<string> authCodeList);


    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<PagedResults<OnlineUserDTO>> GetPageAsync(OnlineUserParam param);



    /// <summary>
    /// 强退
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task DropOutAsync(HashSet<string> ids);

    #endregion
}
