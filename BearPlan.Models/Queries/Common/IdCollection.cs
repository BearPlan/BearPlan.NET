using System.ComponentModel.DataAnnotations;
using BearPlan.Core.Attributes;

namespace BearPlan.Models.Queries.Common;

/// <summary>
/// id模型(log)
/// </summary>
public class IdCollection
{
    /// <summary>
    /// ids
    /// </summary>
    [Display(Name = "Sys_Id")]
    [global::System.ComponentModel.DataAnnotations.Required(ErrorMessage = "{0}required")]
    [AtLeastOneItem]
    public HashSet<long> IdArray { get; set; } = [];
}
