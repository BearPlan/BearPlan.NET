using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Common.Enums;
using BearPlan.Common.MultiLanguage.Resources;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Helper;
using BearPlan.Core.IdGenerator;
using BearPlan.Common.WebApp;
using BearPlan.Core;
using BearPlan.Core.Caches;
using BearPlan.Core.ConfigOptions;
using BearPlan.Core.Utils;
using BearPlan.Infrastructure.Authentication;
using BearPlan.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BearPlan.Common.Global;

namespace BearPlan.Api.Controllers.Auth;

/// <summary>
/// 授权管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class AuthController : BaseApiController
{
    #region 构造函数

    public AuthController(IUserService userService, IPermissionService permissionService,
        IOnlineUserService onlineUserService, IQueuedEmailService queuedEmailService,
        ITokenService tokenService, ITokenBlacklistService tokenBlacklistService,
        IRedisSubscriber redisSubscriber)
    {
        _userService = userService;
        _permissionService = permissionService;
        _onlineUserService = onlineUserService;
        _queuedEmailService = queuedEmailService;
        _tokenService = tokenService;
        _tokenBlacklistService = tokenBlacklistService;
        _redisSubscriber = redisSubscriber;
    }

    #endregion

    #region 字段

    private readonly IUserService _userService;
    private readonly IPermissionService _permissionService;
    private readonly IOnlineUserService _onlineUserService;
    private readonly IQueuedEmailService _queuedEmailService;
    private readonly ITokenService _tokenService;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly IRedisSubscriber _redisSubscriber;

    #endregion

    #region 内部接口

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
    public async Task<CaptchaDTO> CaptchaAsync()
    {
        var showCaptcha = true; //是否显示验证码
        var captchaOptions = App.GetOptions<CaptchaOptions>();
        if (captchaOptions.Threshold > 0)
        {
            var thresholdCacheKey =
                GlobalConstants.CachePrefix.Threshold + App.HttpContext.Connection.RemoteIpAddress;
            var failedThreshold = await App.Cache.GetAsync<int>(thresholdCacheKey);
            if (failedThreshold <= 0)
            {
                failedThreshold = 1;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut), null);
            }

            showCaptcha = failedThreshold > captchaOptions.Threshold;
        }


        var (imgBytes, code) = SixLaborsImageHelper.BuildVerifyCode(captchaOptions.ImgWidth,
            captchaOptions.ImgHeight,
            captchaOptions.FontSize, captchaOptions.KeyLength);
        var img = ImgHelper.ToBase64StringUrl(imgBytes);
        var captchaId = GlobalConstants.CachePrefix.CaptchaId +
                        IdHelper.NextId().ToString().Base64Encode();
        await App.Cache.SetAsync(captchaId, code, TimeSpan.FromMinutes(2), null);
        var model = new CaptchaDTO
        {
            Img = img,
            CaptchaId = captchaId,
            ShowCaptcha = showCaptcha
        };

        return model;
    }


    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
    public async Task<LoginToken> Login([FromBody] LoginParam param, ApiVersion version)
    {
        var loginFailedLimitOptions = App.GetOptions<LoginFailedLimitOptions>();
        var attempsCacheKey = GlobalConstants.CachePrefix.Attempts + App.HttpContext.Connection.RemoteIpAddress +
                              param.UserName;
        LoginAttempt loginAttempt = null;
        if (loginFailedLimitOptions.Enabled)
        {
            loginAttempt = await App.Cache.GetAsync<LoginAttempt>(attempsCacheKey);
            if (loginAttempt.IsNull())
            {
                loginAttempt = new LoginAttempt { Count = 0, IsLocked = false, LockUntil = DateTime.MinValue };
                await App.Cache.SetAsync(attempsCacheKey, loginAttempt,
                    TimeSpan.FromSeconds(loginFailedLimitOptions.Lockout), null);
            }

            if (loginAttempt.IsLocked && DateTime.Now < loginAttempt.LockUntil)
            {
                // 可以实施账户锁定时，通过邮件或短信通知用户。
                // 可以实施账户锁定后要求管理员手动解锁
               throw new BusException(string.Format(Language.Error_AccountLockedWithUnlockTime_Arg0,
                    loginAttempt.LockUntil.ToString("yyyy-MM-dd HH:mm:ss")));
            }
        }


        var captchaOptions = App.GetOptions<CaptchaOptions>();
        var showCaptcha = true; //是否显示验证码
        var thresholdCacheKey = GlobalConstants.CachePrefix.Threshold + App.HttpContext.Connection.RemoteIpAddress;
        var failedThreshold = 0;
        if (captchaOptions.Threshold > 0)
        {
            failedThreshold = await App.Cache.GetAsync<int>(thresholdCacheKey);
            if (failedThreshold <= 0)
            {
                failedThreshold = 1;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut), null);
            }

            showCaptcha = failedThreshold > captchaOptions.Threshold;
        }


        if (App.GetOptions<SystemOptions>().RunMode != RunMode.Dev && showCaptcha)
        {
            if (param.Captcha.IsNullOrEmpty())
            {
               throw new BusException(ValidationError.Required(param, nameof(param.Captcha)));
            }

            if (param.CaptchaId.IsNullOrEmpty())
            {
               throw new BusException(ValidationError.Required(param, nameof(param.CaptchaId)));
            }


            var code = await App.Cache.GetAsync<string>(param.CaptchaId);
            if (code.IsNullOrEmpty())
            {
               throw new BusException(Language.Error_VerificationCodeExpired,450);
            }

            if (!code.Equals(param.Captcha))
            {
                if (captchaOptions.Threshold > 0)
                {
                    failedThreshold++;
                    await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                        TimeSpan.FromSeconds(captchaOptions.TimeOut),
                        null);
                }

               throw new BusException(Language.Error_InvalidVerificationCode, 450);
            }
        }

        var userDto = await _userService.QueryByNameAsync(param.UserName);
        if (userDto == null)
        {
            if (captchaOptions.Threshold > 0)
            {
                failedThreshold++;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut),
                    null);
            }

           throw new BusException(Language.Error_UserNotFound);
        }

        var rsaOptions = App.GetOptions<RsaOptions>();
        var password = new RsaHelper(rsaOptions.PrivateKey, rsaOptions.PublicKey).Decrypt(param.Password);
        if (!BCryptHelper.Verify(password, userDto.Password))
        {
            if (captchaOptions.Threshold > 0)
            {
                failedThreshold++;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut),
                    null);
            }

            if (loginFailedLimitOptions.Enabled && loginAttempt != null)
            {
                loginAttempt.Count++;
                if (loginAttempt.Count >= loginFailedLimitOptions.MaxAttempts)
                {
                    loginAttempt.IsLocked = true;
                    loginAttempt.LockUntil = DateTime.Now.AddSeconds(loginFailedLimitOptions.Lockout);
                }


                await App.Cache.SetAsync(attempsCacheKey, loginAttempt,
                    TimeSpan.FromSeconds(loginFailedLimitOptions.Lockout), null);
            }

            return loginFailedLimitOptions.Enabled
                ? throw new BusException(Language.Error_InvalidPasswordWithLockWarning)
                : throw new BusException(Language.Error_InvalidPassword);
        }

        if (!userDto.Enabled)
        {
            if (captchaOptions.Threshold > 0)
            {
                failedThreshold++;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut),
                    null);
            }

           throw new BusException(Language.Error_UserNotActivated);
        }

        await App.Cache.RemoveAsync(param.CaptchaId);
        await App.Cache.RemoveAsync(thresholdCacheKey);
        await App.Cache.RemoveAsync(attempsCacheKey);
        var netUser = await _userService.GetInfoAsync(userDto.Id);
        var result = await LoginResult(netUser, false, (VersionEnum)version.MajorVersion);
        return result.LoginToken;
    }

    [HttpGet]
    [AllowAnonymous]
    [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
    public async Task<LoginToken> RefreshTokenAsync(string refreshToken, ApiVersion version)
    {

        // refreshToken 为空直接拒绝，避免后续 Redis 查询空 Key
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new BusException("RefreshToken 不存在", httpStatusCode: 403);

        // ========================== 1. Redis 查 refresh 信息 ==========================
        var info = await App.Cache.GetAsync<RefreshTokenCacheModel>(GlobalConstants.CachePrefix.RefreshKey + refreshToken);
        if (info == null)
            throw new BusException("RefreshToken 已失效或不存在", httpStatusCode: 403);

        // ========================== 2. ROTATION：删除旧refreshToken ==========================
        await App.Cache.RemoveAsync($"{GlobalConstants.CachePrefix.OnlineKey}{info.UserId}:{info.ApiVersion}");
        await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.RefreshKey + refreshToken);

        // ========================== 3. 查用户 ==========================
        var user = await _userService.GetInfoAsync(info.UserId);
        if (user == null)
            throw new BusException("用户不存在", httpStatusCode: 401);

        // ========================== 4. LoginResult（传入 deviceId / version） ==========================
        var login = await LoginResult(user, true, info.ApiVersion);
        return login.LoginToken;
    }



    [HttpGet]
    [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
    public async Task<JwtUserInfo> GetInfoAsync()
    {
        var httpUser = App.GetService<IHttpUser>();
        if (httpUser.IsNull() || httpUser.Id == 0)
        {
           throw new BusException(Language.Error_TokenExpired);
        }

        var netUser = await _userService.GetInfoAsync(httpUser.Id);

        var authCodeList = await _permissionService.GetAuthCodeAsync(netUser.Id);


        var jwtUserInfo = await _onlineUserService.CreateJwtUserAsync(netUser, authCodeList);
        return jwtUserInfo;
    }


    ///// <summary>
    ///// 获取邮箱验证码，申请变更邮箱
    ///// </summary>
    ///// <param name="emailCodeDto"></param>
    ///// <returns></returns>
    //[HttpPost]
    //public async Task<ActionResult> ResetEmailCode([FromBody] EmailCodeDto emailCodeDto)
    //{
  

    //    var result = await _queuedEmailService.ResetEmailCode(emailCodeDto.Email, "EmailVerificationCode");
    //    return Ok(result);
    //}


    /// <summary>
    /// 系统用户登出
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
    public async Task LogoutAsync([FromQuery] string refreshToken)
    {
        //清理缓存
        var httpUser = App.GetService<IHttpUser>();
        if (httpUser.IsNotNull())
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.OnlineKey +
                                        httpUser.JwtToken.ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                        httpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserMenuById +
                                        httpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserAuthCodes +
                                        httpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserAuthUrls +
                                        httpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserDataScopeById +
                                        httpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.RefreshKey + refreshToken);
            await _tokenBlacklistService.AddAsync(new Entity.Core.System.TokenBlacklist
            {
                AccessToken = httpUser.JwtToken.ToMd5String16(),
                RefreshToken= refreshToken
            });


        }

    }


    /// <summary>
    /// swagger登录
    /// </summary>
    /// <param name="swaggerLoginParam"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ApiVersion((int)VersionEnum.Def, Deprecated = false)]
    public async Task SwaggerLoginAsync([FromBody] SwaggerLoginParam swaggerLoginParam)
    {
      

        var userDto = await _userService.QueryByNameAsync(swaggerLoginParam.Username);
        if (userDto == null)
        {
            throw new BusException(Language.Error_UserNotFound);
        }

        var rsaOptions = App.GetOptions<RsaOptions>();
        var password =
            new RsaHelper(rsaOptions.PrivateKey, rsaOptions.PublicKey).Decrypt(swaggerLoginParam.Password);
        if (!BCryptHelper.Verify(password, userDto.Password))
        {
            throw new BusException(Language.Error_InvalidPassword);
        }


        if (!userDto.Enabled)
        {
            throw new BusException (Language.Error_UserNotActivated);
        }

        App.HttpContext.Session.SetInt32("swagger-key", 1);
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 登录或刷新token相应结果
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isRefresh"></param>
    /// <param name="deviceId"></param>
    /// <param name="swaggerVersion"></param>
    /// <returns></returns>
    private async Task<LoginDTO> LoginResult(UserInfo user, bool isRefresh, VersionEnum swaggerVersion)
    {
        // ---------------- 登录时加载权限 ----------------
        List<string> authCodes = new();
        if (!isRefresh)
        {
            authCodes = await _permissionService.GetAuthCodeAsync(user.Id);
            authCodes.AddRange(user.Roles.Select(r => r.AuthCode));
        }

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

        // JWT 用户信息（角色 + 权限）
        var jwtUserInfo = await _onlineUserService.CreateJwtUserAsync(user, authCodes);

        // 登录用户基本信息（UserId、TenantId 等）
        var loginUserInfo = await _onlineUserService.SaveLoginUserAsync(jwtUserInfo, remoteIp);

        // ---------------- 设备与平台信息（关键） ----------------
        loginUserInfo.ApiVersion = swaggerVersion;

        // ---------------- 生成 Token（login / refresh） ----------------
        var token = await _tokenService.IssueTokenAsync(loginUserInfo, isRefresh);

        // loginUserInfo 保存 accessToken，用于在线状态
        loginUserInfo.AccessToken = token.AccessToken;
        loginUserInfo.RefreshToken = token.RefreshToken;
        // ---------------- 保存在线状态（按 userId + deviceId 区分） ----------------
        var onlineKey = $"{GlobalConstants.CachePrefix.OnlineKey}{loginUserInfo.UserId}:{loginUserInfo.ApiVersion}";
        await App.Cache.SetAsync(onlineKey, loginUserInfo, TimeSpan.FromHours(2), CacheExpireType.Absolute);

        // ---------------- 返回结果 ----------------
        if (isRefresh)
        {
            return new LoginDTO
            {
                LoginToken = token,
                User = jwtUserInfo
            };
        }
        else
        {
            return new LoginDTO
            {
                LoginToken = token
            };
        }
    }

    #endregion

    #region 微信扫码登录

    /// <summary>
    /// 微信扫码登录 SSE 流：订阅扫码状态变更并实时推送给浏览器
    /// </summary>
    /// <remarks>
    /// 前端用原生 EventSource 连接本接口，收到 data 为状态字符串（waiting/scanned/confirmed/unbound/expired）。
    /// 状态来源：微信扫码回调 → CustomMessageHandler 更新 Redis 状态 + Pub/Sub 通知 → 本接口推送。
    /// </remarks>
    /// <param name="ticket">扫码会话标识（CreateQrcode 返回）</param>
    [HttpGet]
    [AllowAnonymous]
    [NotAudit]
    [NotCors]
    [NotFormatResponse]
    [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
    public async Task QrLoginStream(string ticket)
    {
        if (string.IsNullOrEmpty(ticket))
        {
            throw new BusException("ticket 不能为空");
        }

        Response.ContentType = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";
        // 关闭响应缓冲，保证分块实时写到客户端
        Response.Headers["X-Accel-Buffering"] = "no";

        var statusKey = GlobalConstants.CachePrefix.WeixinScanStatus + ticket;
        var notifyChannel = GlobalConstants.CachePrefix.WeixinScanNotify + ticket;

        // SSE 写入辅助：每条消息按 "data: xxx\n\n" 格式输出并立即刷新
        async Task WriteEventAsync(string data)
        {
            await Response.WriteAsync($"data: {data}\n\n");
            await Response.Body.FlushAsync();
        }

        // 处理状态：确认/未绑定/过期等终态推送后结束连接；非终态仅推送
        async Task<bool> HandleStatusAsync(string status)
        {
            await WriteEventAsync(status);
            return status is nameof(ScanLoginStatus.Confirmed)
                or nameof(ScanLoginStatus.Unbound)
                or nameof(ScanLoginStatus.Expired);
        }

        // 1. 先查当前状态，处理「SSE 建立前已扫码」的竞态
        var current = await App.Cache.GetAsync<string>(statusKey);
        if (current.IsNullOrEmpty())
        {
            // 状态缓存不存在说明二维码已失效或 ticket 非法
            await WriteEventAsync(nameof(ScanLoginStatus.Expired));
            return;
        }

        if (await HandleStatusAsync(current))
        {
            return;
        }

        // 2. 订阅 Redis 通道，监听后续扫码状态变更
        var tcs = new TaskCompletionSource<bool>();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(HttpContext.RequestAborted);
        cts.Token.Register(() => tcs.TrySetResult(false));

        await _redisSubscriber.SubscribeAsync(notifyChannel, async message =>
        {
            // 收到状态变更：终态推送后结束连接
            if (await HandleStatusAsync(message))
            {
                tcs.TrySetResult(true);
            }
        });

        // 3. 等待扫码事件或客户端断开
        await tcs.Task;
    }

    /// <summary>
    /// 微信扫码登录确认：凭 ticket 换取登录 Token
    /// </summary>
    /// <remarks>
    /// 浏览器通过 SSE 收到 confirmed 状态后调用本接口。一次性凭证（weixin:scan:login:{ticket}）读取后立即删除，
    /// 避免凭证被重复使用。
    /// </remarks>
    /// <param name="ticket">扫码会话标识（CreateQrcode 返回）</param>
    [HttpGet]
    [AllowAnonymous]
    [NotAudit]
    [ApiVersion((int)VersionEnum.Pc, Deprecated = false)]
    public async Task<LoginToken> LoginByWeixin(string ticket)
    {
        if (string.IsNullOrEmpty(ticket))
        {
            throw new BusException("ticket 不能为空");
        }

        var loginTicketKey = GlobalConstants.CachePrefix.WeixinScanLogin + ticket;
        var loginTicket = await App.Cache.GetAsync<WeixinScanLoginTicket>(loginTicketKey);
        if (loginTicket == null)
        {
            throw new BusException("扫码登录凭证不存在或已失效");
        }

        // 一次性凭证：读取后立即删除，防止重复换 Token
        await App.Cache.RemoveAsync(loginTicketKey);

        var user = await _userService.GetInfoAsync(loginTicket.UserId);
        if (user == null)
        {
            throw new BusException("用户不存在");
        }

        var result = await LoginResult(user, false, loginTicket.ApiVersion);
        return result.LoginToken;
    }

    #endregion
}
