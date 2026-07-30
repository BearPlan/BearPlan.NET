using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

/// <summary>
/// 客户端运行平台（应用版本分发场景，对应 Node process.platform 取值）
/// </summary>
public enum PlatformEnum
{
    /// <summary>
    /// Windows
    /// </summary>
    [Display(Name = "Windows")]
    Win32 = 0,

    /// <summary>
    /// macOS
    /// </summary>
    [Display(Name = "macOS")]
    Darwin = 1,

    /// <summary>
    /// Linux
    /// </summary>
    [Display(Name = "Linux")]
    Linux = 2,
}
