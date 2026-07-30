using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Log{
    /// <summary>
    /// 作业日志
    /// </summary>
    #region 查询参数
    public class QuartzNetLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(QuartzNetLogDTO), typeof(QuartzNetLog))]
    public class QuartzNetLogDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string TaskName { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string TaskGroup { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务所在类
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ClassName { get; set; }

        /// <summary>
        /// cron 表达式
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Cron { get; set; }

        /// <summary>
        /// 异常详情
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ExceptionDetail { get; set; }

        /// <summary>
        /// 执行耗时（毫秒）
        /// </summary>
        public long ExecutionDuration { get; set; }

        /// <summary>
        /// 执行传参
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string RunParams { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(QuartzNetLogInfo), typeof(QuartzNetLog))]
    public class QuartzNetLogInfo : QuartzNetLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateQuartzNetLogParam), typeof(QuartzNetLog))]
    public class UpdateQuartzNetLogParam : QuartzNetLog
    {
    }
    #endregion
}
