using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

public enum MenuTypeEnum
{
    /// <summary>
    /// 目录
    /// </summary>
    [Display(Name = "目录")]
    Directory = 1,

    /// <summary>
    /// 菜单
    /// </summary>
    [Display(Name = "菜单")]
    Menu = 2,
    /// <summary>
    /// 参数
    /// </summary>
    [Display(Name = "参数")]
    Query = 3
}
