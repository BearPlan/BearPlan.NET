using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission.Role;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 角色接口
/// </summary>
public interface IRoleService : IBaseServices<Role>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<RoleDTO>> GetPageAsync(RoleParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<RoleInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateRoleParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateRoleParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion

    #region 扩展
    /// <summary>
    /// 获取全部角色
    /// </summary>
    Task<List<RoleInfo>> QueryAllAsync();

    /// <summary>
    /// 获取用户角色等级
    /// </summary>
    Task<int?> QueryUserRoleLevelAsync(HashSet<long> ids);

    /// <summary>
    /// 验证角色等级
    /// </summary>
    Task<int> VerificationUserRoleLevelAsync(int? level);

    ///// <summary>
    ///// 更新角色菜单
    ///// </summary>
    //Task UpdateRoleMenuAsync(UpdateRoleMenuParam param);

    ///// <summary>
    ///// 更新角色Apis
    ///// </summary>
    //Task UpdateRoleApiAsync(UpdateRoleApiParam param);
    /// <summary>
    /// 获取权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<RoleApisParam> GetApisAsync(long roleId);
    /// <summary>
    /// 设置权限
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task SetApisAsync(RoleApisParam param);
    #endregion
}
