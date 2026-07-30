using System;
using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Log{
    /// <summary>
    /// 操作日志
    /// </summary>
    #region 查询参数
    public class AuditLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(AuditLogDTO), typeof(AuditLog))]
    public class AuditLogDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 区域
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Area { get; set; }

        /// <summary>
        /// 控制器
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Controller { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Action { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Method { get; set; }

        /// <summary>
        /// /描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 请求url
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RequestParameters { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ResponseData { get; set; }

        /// <summary>
        /// 执行耗时(毫秒)
        /// </summary>
        public long ExecutionDuration { get; set; }

        /// <summary>
        /// 请求IP
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RequestIp { get; set; }

        /// <summary>
        /// IP所属真实地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 用户代理信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string UserAgent { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string OperatingSystem { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string DeviceType { get; set; }

        /// <summary>
        /// 浏览器名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string BrowserName { get; set; }

        /// <summary>
        /// 浏览器版本
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Version { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public new DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(AuditLogInfo), typeof(AuditLog))]
    public class AuditLogInfo : AuditLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateAuditLogParam), typeof(AuditLog))]
    public class UpdateAuditLogParam : AuditLog
    {
    }
    #endregion
}
