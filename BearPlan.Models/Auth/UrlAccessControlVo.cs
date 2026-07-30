

namespace BearPlan.Models.Auth;
/// <summary>
/// 权限
/// </summary>
public class UrlAccessControlVo
{
    /// <summary>
    /// 请求路径
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; set; } = string.Empty;
}
