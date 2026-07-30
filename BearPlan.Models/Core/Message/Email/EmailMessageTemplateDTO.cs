using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Message.Email;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.Message.Email{
    /// <summary>
    /// 邮件模板
    /// </summary>
    #region 查询参数
    public class EmailMessageTemplateParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(EmailMessageTemplateDTO), typeof(EmailMessageTemplate))]
    public class EmailMessageTemplateDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 抄送邮箱地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string BccEmailAddresses { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 邮箱账户标识符
        /// </summary>
        public long EmailAccountId { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(EmailMessageTemplateInfo), typeof(EmailMessageTemplate))]
    public class EmailMessageTemplateInfo : EmailMessageTemplate
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateEmailMessageTemplateParam), typeof(EmailMessageTemplate))]
    public class UpdateEmailMessageTemplateParam : EmailMessageTemplate
    {
    }
    #endregion
}
