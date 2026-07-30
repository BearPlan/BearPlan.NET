using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.System;

/// <summary>
/// 字典查询参数
/// </summary>
public class DictQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 字典名称
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 字典描述
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 类型
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public DictType? DictType { get; set; }
}
