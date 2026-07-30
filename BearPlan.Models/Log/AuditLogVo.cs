using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;

namespace BearPlan.Models.Log;

/// <summary>
/// 审计日志Vo
/// </summary>
[AutoMapping(typeof(AuditLog), typeof(AuditLogVo))]
public class AuditLogVo : BaseEntityDTO<long>
{
    /// <summary>
    /// 区
    /// </summary>
    public string Area { get; set; } = string.Empty;

    /// <summary>
    /// 控制器
    /// </summary>
    public string Controller { get; set; } = string.Empty;

    /// <summary>
    /// 方法
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 请求Url
    /// </summary>
    public string RequestUrl { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数
    /// </summary>
    public string RequestParameters { get; set; } = string.Empty;

    /// <summary>
    /// 相应结果
    /// </summary>
    public string ResponseData { get; set; } = string.Empty;

    /// <summary>
    /// 执行耗时(毫秒)
    /// </summary>
    public int ExecutionDuration { get; set; }

    /// <summary>
    /// 请求IP
    /// </summary>
    public string RequestIp { get; set; } = string.Empty;

    /// <summary>
    /// IP地址
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// 用户代理信息
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// 操作系统
    /// </summary>
    public string OperatingSystem { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型
    /// </summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>
    /// 浏览器名称
    /// </summary>
    public string BrowserName { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; } = string.Empty;
}
