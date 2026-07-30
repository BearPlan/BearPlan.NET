using System;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Queued;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.Queued{
    /// <summary>
    /// 邮件队列
    /// </summary>
    #region 查询参数
    public class QueuedEmailParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(QueuedEmailDTO), typeof(QueuedEmail))]
    public class QueuedEmailDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 发件邮箱
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// 发件人名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string FromName { get; set; }

        /// <summary>
        /// 收件邮箱
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// 收件人名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ToName { get; set; }

        /// <summary>
        /// 回复邮箱
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ReplyTo { get; set; }

        /// <summary>
        /// 回复人名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ReplyToName { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public QueuedEmailPriority Priority { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Cc { get; set; }

        /// <summary>
        /// 密件抄送
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Bcc { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 发送上限次数
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// 是否已发送
        /// </summary>
        public bool IsSend { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 发件邮箱ID
        /// </summary>
        public long EmailAccountId { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(QueuedEmailInfo), typeof(QueuedEmail))]
    public class QueuedEmailInfo : QueuedEmail
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateQueuedEmailParam), typeof(QueuedEmail))]
    public class UpdateQueuedEmailParam : QueuedEmail
    {
    }
    #endregion
}
