using System.Collections.Generic;
using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Entity;
using BearPlan.Entity.Core.Permission.User;
using BearPlan.Entity.Core.Permission;
using SqlSugar;

namespace BearPlan.Entity.Core.Permission.Role
{
    /// <summary>
    /// 角色
    /// </summary>
    [SugarTable("sys_role")]
    [SugarIndex("unique_{table}_Name", nameof(Name), OrderByType.Asc, true)]
    [SugarIndex("unique_{table}_AuthCode", nameof(AuthCode), OrderByType.Asc, true)]
    public class Role : BaseEntity<long>
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

        /// <summary>
        /// 菜单集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleMenu), nameof(RoleMenu.RoleId), nameof(RoleMenu.MenuId))]
        public List<Menu> Menus { get; set; }

        /// <summary>
        /// 部门集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleDept), nameof(RoleDept.RoleId), nameof(RoleDept.DeptId))]
        public List<Dept> Depts { get; set; }

        /// <summary>
        /// 用户列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(UserRole), nameof(UserRole.RoleId), nameof(UserRole.UserId))]
        public List<User.User> Users { get; set; }


        /// <summary>
        /// 菜单集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleApis), nameof(RoleApis.RoleId), nameof(RoleApis.ApisId))]
        public List<Apis> Apis { get; set; }
    }
}
