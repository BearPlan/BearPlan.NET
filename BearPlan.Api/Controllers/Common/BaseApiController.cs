using BearPlan.Core.Global;
using Microsoft.AspNetCore.Authorization;

namespace BearPlan.Api.Controllers.Common;

/// <summary>
/// API对外接口基控制器
/// </summary>
/* 鉴权方式
 * 1. [TokenFilter] 自定义过滤器
 * 2. [AllowAnonymous] 特性 不需要权限访问
 * 3. [Authorize(Roles = "Admin")] .net core 自带的角色授权 表示拥有Admin角色标识即可访问 不会进入AuthorizationHandler
 * 4. [Authorize(Policy = GlobalSwitch.AuthPolicysName)] 自定义策略模式 重写AuthorizationHandler鉴权 配合第四点一起使用
 */
//[TokenFilter]
[Authorize(Policy = AuthConstants.AuthPolicyName)]
public class BaseApiController : BaseController
{
}
