using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

public enum MenuType
{
    /// <summary>
    /// 目录
    /// </summary>
    [Display(Name = "Enum_Menu_Catalog")]
    Catalog = 1,

    /// <summary>
    /// 菜单
    /// </summary>
    [Display(Name = "Enum_Menu_Menu")]
    Menu = 2,

    /// <summary>
    /// 按钮
    /// </summary>
    [Display(Name = "Enum_Menu_Button")]
    Button = 3,

    /// <summary>
    /// 内链
    /// </summary>
    InternalLink = 4,

    /// <summary>
    /// 外链
    /// </summary>
    ExternalLink = 5
}
