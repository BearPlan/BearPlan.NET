using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;

namespace BearPlan.Models.Log;

/// <summary>
/// 任务调度日志Vo
/// </summary>
[AutoMapping(typeof(QuartzNetLog), typeof(QuartzNetLogVo))]
public class QuartzNetLogVo : BaseEntityDTO<long>
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    public string TaskGroup { get; set; } = string.Empty;

    /// <summary>
    /// 程序集名称
    /// </summary>
    public string AssemblyName { get; set; } = string.Empty;

    /// <summary>
    /// 任务所在类
    /// </summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// cron 表达式
    /// </summary>
    public string Cron { get; set; } = string.Empty;

    /// <summary>
    /// 异常详情
    /// </summary>
    public string ExceptionDetail { get; set; } = string.Empty;

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long ExecutionDuration { get; set; }

    /// <summary>
    /// 执行传参
    /// </summary>
    public string RunParams { get; set; } = string.Empty;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }
}
