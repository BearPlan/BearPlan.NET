using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

/// <summary>
/// 应用/插件版本发布状态
/// </summary>
public enum VersionStatusEnum
{
    /// <summary>
    /// 草稿（未发布，作者仍可编辑）
    /// </summary>
    [Display(Name = "草稿")]
    Draft = 0,

    /// <summary>
    /// 开发者预览（仅特定渠道可见，不对外发布）
    /// </summary>
    [Display(Name = "开发者预览")]
    Developer = 1,

    /// <summary>
    /// 已发布（对外可见，允许下载/更新）
    /// </summary>
    [Display(Name = "已发布")]
    Published = 2,

    /// <summary>
    /// 已弃用（不再推荐使用，已发布版本的下线状态）
    /// </summary>
    [Display(Name = "已弃用")]
    Deprecated = 3,
}
