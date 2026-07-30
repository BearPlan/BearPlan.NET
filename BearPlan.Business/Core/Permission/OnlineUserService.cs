using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Pager;
using BearPlan.Common.WebApp;
using BearPlan.Core;
using BearPlan.Core.Caches;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Auth;
using BearPlan.Models.Core.Monitor;
using IP2Region.Net.Abstractions;
using Shyjus.BrowserDetection;
using BearPlan.Common.Global;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 在线用户service
/// </summary>
public class OnlineUserService : IOnlineUserService
{
    #region 字段

    private readonly IBrowserDetector _browserDetector;
    private readonly ISearcher _ipSearcher;
    private readonly ICache _cache;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    #endregion

    #region 构造函数

    /// <summary>
    /// 
    /// </summary>
    /// <param name="browserDetector"></param>
    /// <param name="searcher"></param>
    /// <param name="cache"></param>
    /// <param name="tokenBlacklistService"></param>
    public OnlineUserService(IBrowserDetector browserDetector, ISearcher searcher, ICache cache,
        ITokenBlacklistService tokenBlacklistService)
    {
        _browserDetector = browserDetector;
        _ipSearcher = searcher;
        _cache = cache;
        _tokenBlacklistService = tokenBlacklistService;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 保存在线用户
    /// </summary>
    /// <param name="UserInfo"></param>
    /// <param name="remoteIp"></param>
    public async Task<LoginUserInfo> SaveLoginUserAsync(JwtUserInfo jwtUserInfo, string remoteIp)
    {
        var onlineUser = new LoginUserInfo
        {
            UserId = jwtUserInfo.User.Id,
            UserName = jwtUserInfo.User.UserName,
            NickName = jwtUserInfo.User.NickName,
            DeptId = jwtUserInfo.User.DeptId,
            DeptName = jwtUserInfo.User.Dept?.Name,
            Ip = remoteIp,
            Address = _ipSearcher.Search(remoteIp),
            OperatingSystem = _browserDetector.Browser?.OS,
            DeviceType = _browserDetector.Browser?.DeviceType,
            BrowserName = _browserDetector.Browser?.Name,
            Version = _browserDetector.Browser?.Version,
            //ApiVersion=jwtUserInfo
            LoginTime = DateTime.Now,
            TenantId = jwtUserInfo.User.TenantId,
        };
        return await Task.FromResult(onlineUser);
    }

    /// <summary>
    /// 创建Jwt对象
    /// </summary>
    /// <param name="UserInfo"></param>
    /// <param name="authCodes"></param>
    /// <returns></returns>
    public async Task<JwtUserInfo> CreateJwtUserAsync(UserInfo UserInfo, List<string> authCodes)
    {
        var jwtUser = new JwtUserInfo
        {
            User = UserInfo,
            Roles = UserInfo.Roles.Select(x => x.AuthCode).ToList(), AuthCodes = authCodes
        };
        return await Task.FromResult(jwtUser);
    }


    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public async Task<PagedResults<OnlineUserDTO>> GetPageAsync(OnlineUserParam param)
    {
        // 注意：登录写入缓存时的类型是 LoginUserInfo（见 AuthController/PermissionHandler），
        // 这里必须用 LoginUserInfo 读取，否则 RedisCache 反序列化得到的父类实例
        // 无法强转为子类 OnlineUserDTO，从而抛出 InvalidCastException。
        List<OnlineUserDTO> loginUserInfos = new List<OnlineUserDTO>();
        var arrayList = await _cache.ScriptEvaluateKeys(GlobalConstants.CachePrefix.OnlineKey);
        if (arrayList.Length > 0)
        {
            foreach (var item in arrayList)
            {
                var loginUserInfo = await _cache.GetAsync<LoginUserInfo>(item);
                if (loginUserInfo.IsNull()) continue;
                loginUserInfo.AccessToken = loginUserInfo.AccessToken.ToMd5String16();
                loginUserInfos.Add(App.Mapper.MapTo<OnlineUserDTO>(loginUserInfo));
            }
        }

        List<OnlineUserDTO> newOnlineUsers = new List<OnlineUserDTO>();
        if (loginUserInfos.Count > 0)
        {
            newOnlineUsers = loginUserInfos.Skip((param.PageIndex - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToList();
        }

        return new PagedResults<OnlineUserDTO>
        {
            Data = newOnlineUsers,
            PagerInfo = new PagerInfo(param)
            {

                TotalRowCount = loginUserInfos.Count
            }
        };
    }

    /// <summary>
    /// 强退
    /// </summary>
    /// <param name="ids"></param>
    public async Task DropOutAsync(HashSet<string> ids)
    {
        var list = new List<TokenBlacklist>();
        list.AddRange(ids.Select(x => new TokenBlacklist { AccessToken = x }));
        if (await _tokenBlacklistService.AddAsync(list)>0)
        {
            foreach (var item in ids)
            {
                await _cache.RemoveAsync(GlobalConstants.CachePrefix.OnlineKey + item);
            }
        }
    }

    

    #endregion
}
