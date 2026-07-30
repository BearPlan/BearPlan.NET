using System;
using System.Collections.Generic;
using System.Text;
using BearPlan.Models.Core.Permission.User;

namespace BearPlan.Models.Auth;
/// <summary>
/// JWT令牌用户
/// </summary>
public class JwtUserInfo
{
    /// <summary>
    /// 用户
    /// </summary>
    public UserInfo User { get; set; } = null!;

    /// <summary>
    /// 角色权限
    /// </summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// 按钮权限
    /// </summary>
    public List<string> AuthCodes { get; set; } = [];
}
