using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.Permission;

/// <summary>
/// 菜单查询参数
/// </summary>
public class MenuQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 菜单标题
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 父级ID
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal, IsGreaterThanNumberDefault = false)]
    public long ParentId { get; set; }
}
