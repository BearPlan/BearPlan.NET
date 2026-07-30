using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.System;

/// <summary>
/// 任务调度查询参数
/// </summary>
public class QuartzNetQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 任务名称
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string TaskName { get; set; } = string.Empty;


    /// <summary>
    /// 任务组
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string TaskGroup { get; set; } = string.Empty;


    /// <summary>
    /// 任务描述
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public bool? Enabled { get; set; }
}
