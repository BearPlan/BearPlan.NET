using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.ConfigProvider
{

    /// <summary>
    /// 国际化
    /// </summary>
    [SugarTable("i18n")]
    [ConfigProviderDataBase]
    //[Tenant(AppConfig.TenantTable)]
    public class I18n : BaseEntity<long>
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
}
