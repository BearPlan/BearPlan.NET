using System;
using System.Collections.Generic;
using BearPlan.Common.Enums;
using BearPlan.Core.Entity;
using BearPlan.Entity.Core.Permission.Role;
using SqlSugar;

namespace BearPlan.Entity.Core.Permission
{
    /// <summary>
    /// Api路由
    /// </summary>
    [SugarTable("sys_apis")]
    public class Apis : BaseEntity<Guid>
    {
        /// <summary>
        /// 组
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 路径
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 版本
        /// </summary>
        public VersionEnum Version { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Method { get; set; } = string.Empty;


        /// <summary>
        /// 是否审计
        /// </summary>
        public bool IsAudit { get; set; }

        /// <summary>
        /// 角色集合
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(typeof(RoleApis), nameof(RoleApis.ApisId), nameof(RoleApis.RoleId))]
        public List<Role.Role> Roles { get; set; } = [];
    }
}
