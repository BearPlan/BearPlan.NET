using BearPlan.Core.Attributes;
using SqlSugar;

namespace BearPlan.Models.Queries.System;

/// <summary>
/// 字典详情查询参数
/// </summary>
public class DictDetailQueryCriteria : IConditionalModel
{
    /// <summary>
    /// 字典ID
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public long DictId { get; set; }

    /// <summary>
    /// 标签名
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 数据值
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Value { get; set; } = string.Empty;
}
