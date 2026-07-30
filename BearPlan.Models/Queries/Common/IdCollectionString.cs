using System.ComponentModel.DataAnnotations;
using BearPlan.Core.Attributes;

namespace BearPlan.Models.Queries.Common;

/// <summary>
/// id模型(string)
/// </summary>
public class IdCollectionString
{
    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Sys_Id")]
    [global::System.ComponentModel.DataAnnotations.Required(ErrorMessage = "{0}required")]
    [AtLeastOneItem]
    public HashSet<string> IdArray { get; set; } = [];
}
