using System.ComponentModel.DataAnnotations;

namespace BearPlan.Models.Queries.Login;

/// <summary>
/// Swagger登录用户
/// </summary>
public class SwaggerLoginParam
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Display(Name = "User_Username")]
    [Required(ErrorMessage = "{0}required")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Display(Name = "User_Password")]
    [Required(ErrorMessage = "{0}required")]
    public string Password { get; set; } = string.Empty;
}
