using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BearPlan.Models.Auth;

    /// <summary>
    /// 验证码
    /// </summary>
    public class CaptchaDTO
    {
        /// <summary>
        /// 图片base64
        /// </summary>
        public string Img { get; set; } = string.Empty;

        /// <summary>
        /// 验证码ID
        /// </summary>
        public string CaptchaId { get; set; } = string.Empty;

        /// <summary>
        /// 是否显示验证码
        /// </summary>
        public bool ShowCaptcha { get; set; }
    }

    /// <summary>
    /// 登录用户
    /// </summary>
    public class LoginParam
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "User_Username")]
        [Required(ErrorMessage = "{0}required")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "User_Password")]
        [Required(ErrorMessage = "{0}required")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "Sys_Captcha")]
        public string Captcha { get; set; } = string.Empty;

        /// <summary>
        /// 验证码ID
        /// </summary>
        [Display(Name = "Sys_CaptchaId")]
        public string CaptchaId { get; set; } = string.Empty;
    }
public class LoginDTO
{
    public JwtUserInfo User { get; set; } = null!;
    public LoginToken LoginToken { get; set; } = null!;
}


public class LoginToken
{
    /// <summary>
    /// 授权token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间
    /// </summary>
    public long Expires { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string TokenType { get; set; } = string.Empty;

    /// <summary>
    /// 刷新token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 允许token时间内
    /// </summary>
    public long RefreshTokenExpires { get; set; }
}

/// <summary>
/// 微信扫码登录二维码结果
/// </summary>
public class WeiXinQrCodeDTO
{
    /// <summary>
    /// 本次扫码会话标识（同时作为二维码场景值），用于建立 SSE 连接和换取登录凭证
    /// </summary>
    public string Ticket { get; set; } = string.Empty;

    /// <summary>
    /// 微信二维码 URL，前端渲染为二维码图片
    /// </summary>
    public string Url { get; set; } = string.Empty;
}
