using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.Message;

/// <summary>
/// 邮箱账户查询参数
/// </summary>
public class EmailAccountQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 邮箱
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string UserName { get; set; } = string.Empty;
}
