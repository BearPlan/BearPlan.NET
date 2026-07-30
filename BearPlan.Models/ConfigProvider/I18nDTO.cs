using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.ConfigProvider;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.ConfigProvider
{
    /// <summary>
    /// 国际化
    /// </summary>
    #region 查询参数
    public class I18nParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(I18nDTO), typeof(I18n))]
    public class I18nDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 中文
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string ZhCn { get; set; } = string.Empty;

        /// <summary>
        /// 英文
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string EnUs { get; set; } = string.Empty;

        /// <summary>
        /// 来源
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Source { get; set; }

        /// <summary>
        /// 访问次数
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(I18nInfo), typeof(I18n))]
    public class I18nInfo : I18n
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateI18nParam), typeof(I18n))]
    public class UpdateI18nParam : I18n
    {
    }
    #endregion
}
