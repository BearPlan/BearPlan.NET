using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

/// <summary>
/// CPU 架构（应用版本分发场景，对应 Node process.arch 取值）
/// </summary>
public enum ArchEnum
{
    /// <summary>
    /// x64（Intel/AMD 64 位）
    /// </summary>
    [Display(Name = "x64")]
    X64 = 0,

    /// <summary>
    /// ARM64（Apple Silicon / 鲲鹏等）
    /// </summary>
    [Display(Name = "ARM64")]
    Arm64 = 1,

    /// <summary>
    /// x86（32 位，兼容旧设备）
    /// </summary>
    [Display(Name = "x86")]
    X86 = 2,
}
