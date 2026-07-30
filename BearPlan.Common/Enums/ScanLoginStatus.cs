namespace BearPlan.Common.Enums;

/// <summary>
/// 微信扫码登录状态机
/// </summary>
public enum ScanLoginStatus
{
    /// <summary>
    /// 等待扫码（二维码已生成，尚未被微信扫描）
    /// </summary>
    Waiting,

    /// <summary>
    /// 已扫码（用户已扫码，但该微信号未绑定系统账号，无法自动登录）
    /// </summary>
    Unbound,

    /// <summary>
    /// 已确认登录（扫码成功且微信号已绑定系统账号，可换取 Token）
    /// </summary>
    Confirmed,

    /// <summary>
    /// 已过期（二维码或会话已失效，需重新生成）
    /// </summary>
    Expired
}
