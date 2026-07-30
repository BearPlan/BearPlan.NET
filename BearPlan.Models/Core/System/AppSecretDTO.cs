using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.System{
    /// <summary>
    /// 三方应用密钥
    /// </summary>
    #region 查询参数
    public class AppSecretParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(AppSecretDTO), typeof(AppSecret))]
    public class AppSecretDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// 应用秘钥
        /// </summary>
        public string AppSecretKey { get; set; } = string.Empty;

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(AppSecretInfo), typeof(AppSecret))]
    public class AppSecretInfo : AppSecret
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateAppSecretParam), typeof(AppSecret))]
    public class UpdateAppSecretParam : AppSecret
    {
    }
    #endregion
}
