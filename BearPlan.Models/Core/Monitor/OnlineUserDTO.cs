using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Common.WebApp;

namespace BearPlan.Models.Core.Monitor;

/// <summary>
/// 在线用户服务
/// </summary>
#region 查询参数
public class OnlineUserParam : PageParam
{

}
#endregion
#region DTO
/// <summary>
///  分页
/// </summary>
[AutoMapping(typeof(OnlineUserDTO), typeof(LoginUserInfo))]
public class OnlineUserDTO : LoginUserInfo
{
}
#endregion
