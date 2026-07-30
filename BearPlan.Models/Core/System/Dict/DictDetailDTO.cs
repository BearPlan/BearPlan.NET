using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System.Dict;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.System.Dict{
    /// <summary>
    /// 字典详情
    /// </summary>
    #region 查询参数
    public class DictDetailParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(DictDetailDTO), typeof(DictDetail))]
    public class DictDetailDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 字典ID
        /// </summary>
        public long DictId { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// 字典值
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 排序
        /// </summary>
        public int DictSort { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(DictDetailInfo), typeof(DictDetail))]
    public class DictDetailInfo : DictDetail
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateDictDetailParam), typeof(DictDetail))]
    public class UpdateDictDetailParam : DictDetail
    {
    }
    #endregion
}
