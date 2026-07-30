using BearPlan.Common.WebApp;
using BearPlan.Models.Auth;

namespace BearPlan.Infrastructure.Authentication;

public interface ITokenService
{
    /// <summary>
    /// 颁发Token
    /// </summary>
    /// <param name="loginUserInfo"></param>
    /// <param name="refresh"></param>
    /// <param name="refreshTime"></param>
    /// <returns></returns>
    Task<LoginToken> IssueTokenAsync(LoginUserInfo loginUserInfo, bool refresh = false, long refreshTime = 0);


}
