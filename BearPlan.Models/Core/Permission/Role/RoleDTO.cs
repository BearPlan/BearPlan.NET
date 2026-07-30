using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using RoleEntity = BearPlan.Entity.Core.Permission.Role.Role;
using BearPlan.Models.Common;
using BearPlan.Core.Pager;

namespace BearPlan.Models.Core.Permission.Role{
    /// <summary>
    /// 角色
    /// </summary>
    #region 查询参数
    public class RoleParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(RoleDTO), typeof(RoleEntity))]
    public class RoleDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据权限
        /// </summary>
        public DataScopeType DataScopeType { get; set; }

        /// <summary>
        /// 角色代码
        /// </summary>
        public string AuthCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(RoleInfo), typeof(RoleEntity))]
    public class RoleInfo : RoleEntity
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateRoleParam), typeof(RoleEntity))]
    public class UpdateRoleParam : RoleEntity
    {
        /// <summary>
        /// 部门ID列表（DataScopeType=Customize时使用）
        /// </summary>
        public List<long> DeptIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// 更新角色菜单关联参数
    /// </summary>
    public class UpdateRoleMenuParam
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 菜单ID列表
        /// </summary>
        public List<long> MenuIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// 更新角色Api关联参数
    /// </summary>
    public class UpdateRoleApiParam
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Api ID列表
        /// </summary>
        public List<Guid> ApiIds { get; set; } = new List<Guid>();
    }
    #endregion

    public class RoleApisParam
    {
        public long RoleId { get; set; }

        public List<long> MenuIds { get; set; } = [];

        public Dictionary<int, List<Guid>> ApiIds { get; set; } = new Dictionary<int, List<Guid>>();
    }
}
