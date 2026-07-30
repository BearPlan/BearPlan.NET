using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.System{
    /// <summary>
    /// 租户
    /// </summary>
    #region 查询参数
    public class TenantParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(TenantDTO), typeof(Tenant))]
    public class TenantDTO : RootKeyDTO<long>
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
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 租户类型
        /// </summary>
        public TenantType TenantType { get; set; }

        /// <summary>
        /// 库Id
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ConfigId { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public SqlSugar.DbType? DbType { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(TenantInfo), typeof(Tenant))]
    public class TenantInfo : Tenant
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateTenantParam), typeof(Tenant))]
    public class UpdateTenantParam : Tenant
    {
    }
    #endregion
}
