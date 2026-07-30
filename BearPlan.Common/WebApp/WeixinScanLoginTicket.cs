using BearPlan.Common.Enums;

namespace BearPlan.Common.WebApp;

/// <summary>
/// 微信扫码登录一次性凭证（缓存于 weixin:scan:login:{ticket}，换 Token 后立即删除）
/// </summary>
public class WeixinScanLoginTicket
{
    /// <summary>
    /// 已绑定的系统用户 Id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 登录平台（对应 URL 版本号）
    /// </summary>
    public VersionEnum ApiVersion { get; set; }
}
