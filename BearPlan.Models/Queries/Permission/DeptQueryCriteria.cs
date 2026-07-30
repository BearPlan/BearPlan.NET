using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.Permission;

/// <summary>
/// 部门查询参数
/// </summary>
public class DeptQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public bool? Enabled { get; set; }

    /// <summary>
    /// 父级ID
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal, IsGreaterThanNumberDefault = false)]
    public long ParentId { get; set; }
}
