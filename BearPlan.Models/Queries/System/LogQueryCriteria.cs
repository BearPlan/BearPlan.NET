using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.System;

/// <summary>
/// 日志查询参数
/// </summary>
public class LogQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 描述
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string CreateBy { get; set; } = string.Empty;
}
