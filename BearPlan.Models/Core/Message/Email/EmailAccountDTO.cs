using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Message.Email;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.Message.Email{
    /// <summary>
    /// 邮件账户
    /// </summary>
    #region 查询参数
    public class EmailAccountParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(EmailAccountDTO), typeof(EmailAccount))]
    public class EmailAccountDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 电子邮件地址
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮件显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮件主机
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮件端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 电子邮件用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮件密码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Password { get; set; }

        /// <summary>
        /// 是否SSL
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 是否与请求一起发送应用程序的默认系统凭据
        /// </summary>
        public bool UseDefaultCredentials { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(EmailAccountInfo), typeof(EmailAccount))]
    public class EmailAccountInfo : EmailAccount
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateEmailAccountParam), typeof(EmailAccount))]
    public class UpdateEmailAccountParam : EmailAccount
    {
    }
    #endregion
}
