using System;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Log{
    /// <summary>
    /// 异常日志
    /// </summary>
    #region 查询参数
    public class ExceptionLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(ExceptionLogDTO), typeof(ExceptionLog))]
    public class ExceptionLogDTO : RootKeyDTO<long>
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
        /// 描述
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
        /// 异常短信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 异常完整信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ExceptionMessageFull { get; set; }

        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ExceptionStack { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 请求ip
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RequestIp { get; set; }

        /// <summary>
        /// ip所属真实地址
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
        ///
        /// </summary>
        public new DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(ExceptionLogInfo), typeof(ExceptionLog))]
    public class ExceptionLogInfo : ExceptionLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateExceptionLogParam), typeof(ExceptionLog))]
    public class UpdateExceptionLogParam : ExceptionLog
    {
    }
    #endregion
}
