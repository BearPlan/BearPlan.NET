using System.Collections.Generic;
using BearPlan.Core.Enums;
using BearPlan.Core.Entity;
using BearPlan.Entity.Core.Permission.User;
using SqlSugar;

namespace BearPlan.Entity.Core.System
{
    /// <summary>
    /// 租户
    /// </summary>
    [SugarTable("sys_tenant")]
    public class Tenant : BaseEntity<long>
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 租户类型
        /// </summary>
        public TenantType TenantType { get; set; }

        /// <summary>
        /// 库Id
        /// </summary>
        public string ConfigId { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public string ConnectionString { get; set; }


        #region 扩展属性

        /// <summary>
        /// 用户列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(User.TenantId), nameof(TenantId))]
        public List<User> Users { get; set; }

        #endregion
    }
}
