using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.System{
    /// <summary>
    /// 参数配置
    /// </summary>
    #region 查询参数
    public class SettingParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(SettingDTO), typeof(Setting))]
    public class SettingDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(SettingInfo), typeof(Setting))]
    public class SettingInfo : Setting
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateSettingParam), typeof(Setting))]
    public class UpdateSettingParam : Setting
    {
    }
    #endregion
}
