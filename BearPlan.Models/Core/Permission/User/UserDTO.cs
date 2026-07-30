using System;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using UserEntity = BearPlan.Entity.Core.Permission.User.User;
using BearPlan.Models.Common;
using BearPlan.Core.Pager;

namespace BearPlan.Models.Core.Permission.User{
    /// <summary>
    /// 系统用户
    /// </summary>
    #region 查询参数
    public class UserParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(UserDTO), typeof(UserEntity))]
    public class UserDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; }

       

        /// <summary>
        /// 部门
        /// </summary>
        public long DeptId { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 最后修改密码时间
        /// </summary>
        public DateTime? PasswordReSetTime { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public SexEnum Sex { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 界面偏好配置
        /// </summary>
        public string PreferencesConfig { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(UserInfo), typeof(UserEntity))]
    public class UserInfo : UserEntity
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateUserParam), typeof(UserEntity))]
    public class UpdateUserParam : UserEntity
    {
        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<long> RoleIds { get; set; } = new List<long>();

        /// <summary>
        /// 岗位ID列表
        /// </summary>
        public List<long> JobIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// 更新用户中心信息参数
    /// </summary>
    public class UpdateUserCenterParam
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        public SexEnum Sex { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; } = string.Empty;
    }

    /// <summary>
    /// 修改密码参数
    /// </summary>
    public class UpdateUserPassParam
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// 修改邮箱参数
    /// </summary>
    public class UpdateUserEmailParam
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新偏好配置参数
    /// </summary>
    public class UpdateUserPreferencesConfigParam
    {
        /// <summary>
        /// 偏好配置JSON
        /// </summary>
        public string PreferencesConfig { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新用户角色参数
    /// </summary>
    public class UpdateUserRoleParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<long> RoleIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// 更新用户岗位参数
    /// </summary>
    public class UpdateUserJobParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 岗位ID列表
        /// </summary>
        public List<long> JobIds { get; set; } = new List<long>();
    }
    #endregion
}
