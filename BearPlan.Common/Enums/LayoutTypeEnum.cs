using System.ComponentModel.DataAnnotations;

namespace BearPlan.Common.Enums;

public enum LayoutTypeEnum
{
    [Display(Name = "layout.base")]
    Base = 1,
    [Display(Name = "layout.blank")]
    Blank = 2,
}
