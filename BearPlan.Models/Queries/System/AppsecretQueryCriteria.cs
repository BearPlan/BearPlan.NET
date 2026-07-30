using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.System;

/// <summary>
/// 密钥查询参数
/// </summary>
public class AppsecretQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 应用Id
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public long AppId { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string AppName { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Remark { get; set; } = string.Empty;
}
