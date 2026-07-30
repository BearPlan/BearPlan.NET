using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Pager;
using DictEntity = BearPlan.Entity.Core.System.Dict.Dict;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.System.Dict{
    /// <summary>
    /// 字典
    /// </summary>
    #region 查询参数
    public class DictParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(DictDTO), typeof(DictEntity))]
    public class DictDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 字典类型
        /// </summary>
        public DictType DictType { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(DictInfo), typeof(DictEntity))]
    public class DictInfo : DictEntity
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateDictParam), typeof(DictEntity))]
    public class UpdateDictParam : DictEntity
    {
    }
    #endregion
}
