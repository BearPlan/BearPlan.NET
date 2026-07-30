using System;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.System{
    /// <summary>
    /// 系统作业调度
    /// </summary>
    #region 查询参数
    public class QuartzNetParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(QuartzNetDTO), typeof(QuartzNet))]
    public class QuartzNetDTO : RootKeyDTO<long>
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
        [SugarColumn(IsNullable = true)]
        public string Cron { get; set; }

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
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 任务负责人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Principal { get; set; }

        /// <summary>
        /// 告警邮箱
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string AlertEmail { get; set; }

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
        [SugarColumn(IsNullable = true)]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 触发器类型（0、simple 1、cron）
        /// </summary>
        public TriggerType TriggerType { get; set; }

        /// <summary>
        /// 执行间隔时间, 秒为单位
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? IntervalSecond { get; set; }

        /// <summary>
        /// 循环执行次数
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? CycleRunTimes { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RunParams { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(QuartzNetInfo), typeof(QuartzNet))]
    public class QuartzNetInfo : QuartzNet
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateQuartzNetParam), typeof(QuartzNet))]
    public class UpdateQuartzNetParam : QuartzNet
    {
    }
    #endregion
}
