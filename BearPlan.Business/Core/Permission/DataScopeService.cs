using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Entity.Core.Permission;
using BearPlan.Entity.Core.Permission.Role;
using BearPlan.Entity.Core.Permission.User;
using BearPlan.IBusiness;
using SqlSugar;
using Enumerable = System.Linq.Enumerable;
using BearPlan.Common.Global;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 数据权限服务
/// </summary>
public class DataScopeService : IDataScopeService
{
    #region 字段

    private readonly ISqlSugarClient _db;

    #endregion

    #region 构造函数

    /// <summary>
    /// 
    /// </summary>
    /// <param name="db"></param>
    public DataScopeService(ISqlSugarClient db)
    {
        _db = db;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 获取指定用户在数据权限范围内可访问的所有用户ID；
    /// 返回集合仅含 <see cref="IDataScopeService.AllScopeId"/> 表示全部权限，空集合表示仅本人
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [UseCache(Expiration = 60, KeyPrefix = GlobalConstants.CachePrefix.UserDataScopeById)]
    public async Task<List<long>> GetDataScopeUserIdsAsync(long userId)
    {
        var user = await _db.Queryable<User>().Includes(x => x.Roles).FirstAsync(x => x.Id == userId);
        if (user.IsNull())
        {
            return new List<long>();
        }

        var isAll = Enumerable.Any(user.Roles, x => x.DataScopeType == DataScopeType.All);

        if (isAll)
        {
            return [IDataScopeService.AllScopeId];
        }

        var idList = new List<long>();
        foreach (var role in user.Roles)
        {
            idList.AddRange(await GetUserIds(role.DataScopeType, role.Id, user.DeptId, user.Id));
        }

        return Enumerable.ToList(Enumerable.Distinct(idList));
    }


    /// <summary>
    /// 按数据权限类型获取可见用户ID
    /// </summary>
    /// <param name="dataScopeType"></param>
    /// <param name="roleId"></param>
    /// <param name="deptId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<List<long>> GetUserIds(DataScopeType dataScopeType, long roleId, long deptId, long userId)
    {
        List<long> idList = new List<long>();
        switch (dataScopeType)
        {
            case DataScopeType.MySelf:
                idList.Add(userId);
                break;
            case DataScopeType.MyDept:
                {
                    var userList = await _db.Queryable<User>().Where(x => x.DeptId == deptId).ToListAsync();
                    idList.AddRange(Enumerable.Select(userList, x => x.Id));
                    break;
                }
            case DataScopeType.MyDeptAndBelow:
                {
                    var deptIds = await GetChildIds([deptId], null);
                    var userList = await _db.Queryable<User>().Where(x => deptIds.Contains(x.DeptId)).ToListAsync();
                    idList.AddRange(Enumerable.Select(userList, x => x.Id));
                    break;
                }
            case DataScopeType.Customize:
                {
                    var role = await _db.Queryable<Role>().Includes(x => x.Depts).Where(x => x.Id == roleId)
                        .FirstAsync();
                    if (role.IsNotNull())
                    {
                        var deptIds = role.Depts.Select(x => x.Id).ToList();
                        var userList = await _db.Queryable<User>().Where(x => deptIds.Contains(x.DeptId)).ToListAsync();
                        idList.AddRange(userList.Select(x => x.Id));
                    }

                    break;
                }
        }

        return idList;
    }

    private async Task<List<long>> GetChildIds(List<long> ids, List<long> allIds)
    {
        allIds ??= new List<long>();

        foreach (var id in ids.Where(id => !allIds.Contains(id)))
        {
            allIds.Add(id);
            var list = await _db.Queryable<Dept>().Where(x => x.ParentId == id && x.Enabled).ToListAsync();
            if (list.Any())
            {
                await GetChildIds(list.Select(x => x.Id).ToList(), allIds);
            }
        }

        return allIds;
    }

    #endregion
}
