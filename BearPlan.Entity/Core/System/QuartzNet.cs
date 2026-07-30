using System;
using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Entity;
using Quartz;
using SqlSugar;

namespace BearPlan.Entity.Core.System
{
    /// <summary>
    /// 系统作业调度
    /// </summary>
    [SugarTable("sys_quartz_job")]
    public class QuartzNet : BaseEntity<long>
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// 任务分组
        /// </summary>
        public string TaskGroup { get; set; } = string.Empty;

        /// <summary>
        /// cron 表达式
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Cron { get; set; } = string.Empty;

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; set; } = string.Empty;

        /// <summary>
        /// 任务所在类
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// 任务描述
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 任务负责人
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Principal { get; set; } = string.Empty;

        /// <summary>
        /// 告警邮箱
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string AlertEmail { get; set; } = string.Empty;

        /// <summary>
        /// 任务失败后是否暂停
        /// </summary>
        public bool PauseAfterFailure { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int RunTimes { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 触发器类型（0、simple 1、cron）
        /// </summary>
        public TriggerType TriggerType { get; set; }

        /// <summary>
        /// 执行间隔时间, 秒为单位
        /// </summary>
        public int? IntervalSecond { get; set; }

        /// <summary>
        /// 循环执行次数
        /// </summary>
        public int? CycleRunTimes { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string RunParams { get; set; } = string.Empty;

        /// <summary>
        /// 触发器状态
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public TriggerState TriggerState { get; set; }
    }
}
