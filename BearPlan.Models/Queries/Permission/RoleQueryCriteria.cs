using BearPlan.Core.Attributes;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Models.Queries.Permission;

/// <summary>
/// 角色查询参数
/// </summary>
public class RoleQueryCriteria : DateRange, IConditionalModel
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Like)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据权限类型
    /// </summary>
    [QueryCondition(ConditionType = ConditionalType.Equal)]
    public int DataScopeType { get; set; }
}
