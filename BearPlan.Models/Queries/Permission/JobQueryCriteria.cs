using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.Permission;

/// <summary>
/// 岗位查询参数
/// </summary>
public class JobQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 岗位名称
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public bool? Enabled { get; set; }
}
