using System.Collections.Generic;
using System.Threading.Tasks;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 数据权限接口
/// </summary>
public interface IDataScopeService
{
    /// <summary>
    /// 全部数据权限的哨兵ID：返回集合仅含该值时表示不限数据范围
    /// </summary>
    const long AllScopeId = -1;

    /// <summary>
    /// 获取指定用户在数据权限范围内可访问的所有用户ID；
    /// 返回集合仅含 <see cref="AllScopeId"/> 表示全部权限，空集合表示仅本人
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<long>> GetDataScopeUserIdsAsync(long userId);
}
