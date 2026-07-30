using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

public enum DictType
{
    /// <summary>
    /// 系统类
    /// </summary>
    [Display(Name = "Enum_Dict_System")]
    System = 1,

    /// <summary>
    /// 业务类
    /// </summary>
    [Display(Name = "Enum_Dict_Business")]
    Business = 2
}
