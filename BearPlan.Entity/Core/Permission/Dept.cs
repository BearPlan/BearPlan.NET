using System.Collections.Generic;
using BearPlan.Core.Entity;
using BearPlan.Entity.Core.Permission.Role;
using SqlSugar;

namespace BearPlan.Entity.Core.Permission
{
    /// <summary>
    /// 部门
    /// </summary>
    [SugarTable("sys_dept")]
    public class Dept : BaseEntity<long>
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 父级部门ID
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(DefaultValue = "999")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 子节点个数
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int SubCount { get; set; }


        /// <summary>
        /// 用户列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(User.User.DeptId), nameof(Id))]
        public List<User.User> Users { get; set; }

        /// <summary>
        /// 用户集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleDept), nameof(RoleDept.DeptId), nameof(RoleDept.RoleId))]
        public List<Role.Role> Roles { get; set; }
    }
}
