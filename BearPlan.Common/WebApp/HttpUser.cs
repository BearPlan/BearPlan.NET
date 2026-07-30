using System;
using System.Linq;
using BearPlan.Common.Enums;
using BearPlan.Core.Global;
using Microsoft.AspNetCore.Http;

namespace BearPlan.Common.WebApp;

public class HttpUser : IHttpUser
{
    private readonly HttpContext _httpContext;

    public HttpUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor?.HttpContext;
    }

    #region 登录ID

    /// <summary>
    /// 登录ID
    /// </summary>
    public long Id
    {
        get
        {
            if (IsAuthenticated)
            {
                var claim = _httpContext?.User.Claims.FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Jti);
                return Convert.ToInt64(claim?.Value);
            }

            return default;
        }
    }

    #endregion

    #region 登录账号

    /// <summary>
    /// 登录账号
    /// </summary>
    public string Account
    {
        get
        {
            if (IsAuthenticated)
            {
                var claim = _httpContext?.User.Claims.FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Name);
                return claim?.Value;
                //return _httpContext?.User.Identity?.Name;
            }

            return string.Empty;
        }
    }

    #endregion

    #region 部门ID

    /// <summary>
    /// 部门ID
    /// </summary>
    public long DeptId
    {
        get
        {
            if (IsAuthenticated)
            {
                var claim = _httpContext?.User.Claims.FirstOrDefault(
                    s => s.Type == AuthConstants.JwtClaimTypes.DeptId);
                return Convert.ToInt64(claim?.Value);
            }

            return default;
        }
    }

    #endregion

    #region 租户ID

    /// <summary>
    /// 租户ID
    /// </summary>
    public int TenantId
    {
        get
        {
            if (IsAuthenticated)
            {
                var claim = _httpContext?.User.Claims.FirstOrDefault(
                    s => s.Type == AuthConstants.JwtClaimTypes.TenantId);
                return string.IsNullOrWhiteSpace(claim?.Value) ? 0 : Convert.ToInt32(claim.Value);
            }

            return default;
        }
    }

    #endregion

    #region jwt token

    /// <summary>
    /// jwt token
    /// </summary>
    /// <returns></returns>
    public string JwtToken
    {
        get
        {
            if (IsAuthenticated && _httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return _httpContext?.Request.Headers["Authorization"].ToString()
                    .Replace(AuthConstants.JwtTokenType, "").Trim();
            }

            return string.Empty;
        }
    }

   
    #endregion

    #region 是否已认证

    /// <summary>
    /// 是否已认证
    /// </summary>
    /// <returns></returns>
    public bool IsAuthenticated => _httpContext?.User.Identity?.IsAuthenticated ?? false;

    #endregion

    #region Api版本
    public VersionEnum ApiVersion
    {
        get
        {
            if (IsAuthenticated)
            {
                // First, check JWT claim
                var claim = _httpContext?.User.Claims.FirstOrDefault(
                    s => s.Type == AuthConstants.JwtClaimTypes.ApiVersion);

                if (claim != null && Enum.TryParse(claim.Value, out VersionEnum version))
                {
                    return version;
                }

                // Optionally check the api-version from the header
                var apiVersionFromHeader = _httpContext?.Request.Headers["api-version"].FirstOrDefault();
                if (apiVersionFromHeader != null && Enum.TryParse(apiVersionFromHeader, out VersionEnum headerVersion))
                {
                    return headerVersion;
                }

                // Default if no valid version is found
                return VersionEnum.Def;
            }

            // Default to Def if not authenticated
            return VersionEnum.Def;
        }
    }
    #endregion
}
