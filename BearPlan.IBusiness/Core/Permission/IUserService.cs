using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission.User;
using BearPlan.Models;
using Microsoft.AspNetCore.Http;

namespace BearPlan.IBusiness.Core.Permission;

/// <summary>
/// 用户接口
/// </summary>
public interface IUserService : IBaseServices<User>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<UserDTO>> GetPageAsync(UserParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<UserInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateUserParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateUserParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion

    #region 扩展

    /// <summary>
    /// 查找用户
    /// </summary>
    Task<UserInfo> QueryByNameAsync(string userName);

    /// <summary>
    /// 根据部门ID查找用户
    /// </summary>
    Task<List<UserDTO>> QueryByDeptIdsAsync(List<long> deptIds);

    /// <summary>
    /// 修改个人中心信息
    /// </summary>
    Task UpdateCenterAsync(UpdateUserCenterParam param);

    /// <summary>
    /// 修改密码
    /// </summary>
    Task UpdatePasswordAsync(UpdateUserPassParam param);

    /// <summary>
    /// 修改偏好配置
    /// </summary>
    Task UpdatePreferencesConfigAsync(UpdateUserPreferencesConfigParam param);

    /// <summary>
    /// 修改邮箱
    /// </summary>
    Task UpdateEmailAsync(UpdateUserEmailParam param);

    /// <summary>
    /// 修改头像
    /// </summary>
    Task<string> UpdateAvatarAsync(IFormFile file);

    /// <summary>
    /// 修改用户角色
    /// </summary>
    Task UpdateUserRoleAsync(UpdateUserRoleParam param);

    /// <summary>
    /// 修改用户岗位
    /// </summary>
    Task UpdateUserJobAsync(UpdateUserJobParam param);
    #endregion
}
