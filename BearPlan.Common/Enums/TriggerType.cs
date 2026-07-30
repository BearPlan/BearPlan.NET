using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

public enum TriggerType
{
    /// <summary>
    /// 表达式
    /// </summary>
    [Display(Name = "Enum_Trigger_Cron")]
    Cron = 1,

    /// <summary>
    /// 简单的
    /// </summary>
    [Display(Name = "Enum_Trigger_Simple")]
    Simple = 2
}
