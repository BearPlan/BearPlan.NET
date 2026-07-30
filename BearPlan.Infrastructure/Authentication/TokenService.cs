using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BearPlan.Core.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Common.WebApp;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Models.Auth;
using Dm.util;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using BearPlan.Common.Global;

namespace BearPlan.Infrastructure.Authentication;

/// <summary>
/// 
/// </summary>
public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;


    public TokenService(ILogger<TokenService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 颁发Token
    /// </summary>
    /// <param name="loginUserInfo"></param>
    /// <param name="refresh"></param>
    /// <param name="refreshTime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<LoginToken> IssueTokenAsync(LoginUserInfo loginUserInfo, bool refresh = false, long refreshTime = 0)
    {
        if (loginUserInfo == null)
            throw new ArgumentNullException(nameof(loginUserInfo));

        var jwtAuthOptions = App.GetOptions<JwtAuthOptions>();
        var signinCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.SecurityKey)),
                SecurityAlgorithms.HmacSha256);
        var nowTime = DateTime.Now;
        if (refreshTime == 0)
        {
            refreshTime = nowTime.AddHours(jwtAuthOptions.RefreshTokenExpires).ToUnixTimeStampMillisecond();
        }

        var cls = new List<Claim>
        {
            new(AuthConstants.JwtClaimTypes.Jti, loginUserInfo.UserId.ToString()),
            new(AuthConstants.JwtClaimTypes.Name, loginUserInfo.UserName),
            new(AuthConstants.JwtClaimTypes.TenantId, loginUserInfo.TenantId.ToString()),
            new(AuthConstants.JwtClaimTypes.DeptId, loginUserInfo.DeptId.ToString()),
            new(AuthConstants.JwtClaimTypes.Iat, nowTime.ToUnixTimeStampMillisecond().ToString()),
            new(AuthConstants.JwtClaimTypes.Ip, loginUserInfo.Ip),
            new(AuthConstants.JwtClaimTypes.RefreshTime, refreshTime.toString()),
            new(AuthConstants.JwtClaimTypes.ApiVersion, loginUserInfo.ApiVersion.ToString()),
        };
        var identity = new ClaimsIdentity(AuthConstants.JwtTokenType);
        identity.AddClaims(cls);


        var tokeOptions = new JwtSecurityToken(
            issuer: jwtAuthOptions.Issuer,
            audience: jwtAuthOptions.Audience,
            claims: cls,
            notBefore: nowTime,
            expires: nowTime.AddHours(jwtAuthOptions.Expires),
            signingCredentials: signinCredentials
        );

        var expires = nowTime.AddHours(jwtAuthOptions.Expires).ToUnixTimeStampMillisecond();
        var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        var refreshToken = Guid.NewGuid().ToString("N");
        if (refresh)
        {

                 await App.Cache.SetAsync(
                  GlobalConstants.CachePrefix.RefreshKey + refreshToken,
                new RefreshTokenCacheModel
                {
                    UserId = loginUserInfo.UserId,
                    ApiVersion = loginUserInfo.ApiVersion,
                    ExpiresAt = expires
                },
                TimeSpan.FromSeconds(jwtAuthOptions.RefreshTokenExpires * 3600),
                CacheExpireType.Absolute
            );


            return await Task.FromResult(new LoginToken
            {
                AccessToken = token,
                Expires = expires,
                TokenType = AuthConstants.JwtTokenType,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTime
            });
        }


        // ----------------- Login Mode -----------------
        await App.Cache.SetAsync(
                  GlobalConstants.CachePrefix.RefreshKey + refreshToken,
                  new RefreshTokenCacheModel
                  {
                      UserId = loginUserInfo.UserId,
                      ApiVersion = loginUserInfo.ApiVersion,
                      ExpiresAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + jwtAuthOptions.RefreshTokenExpires * 3600L
                  },
                  TimeSpan.FromSeconds(jwtAuthOptions.RefreshTokenExpires * 3600),
                  CacheExpireType.Absolute
         );
        return await Task.FromResult(new LoginToken
        {
            AccessToken = token,
            Expires = expires,
            TokenType = AuthConstants.JwtTokenType,
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshTime
        });
    }
}
