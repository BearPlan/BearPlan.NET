using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.System;

/// <summary>
/// 文件记录查询参数
/// </summary>
public class FileRecordQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 关键字
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Description { get; set; } = string.Empty;
}
