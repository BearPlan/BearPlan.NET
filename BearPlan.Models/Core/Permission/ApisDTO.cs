using BearPlan.Core.Attributes;
using BearPlan.Common.Enums;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models.Common;

namespace BearPlan.Models.Core.Permission{
    /// <summary>
    /// Api路由
    /// </summary>
    #region 查询参数
    public class ApisParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(ApisDTO), typeof(Apis))]
    public class ApisDTO : RootKeyDTO<Guid>
    {
        /// <summary>
        /// 组
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 路径
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public string Method { get; set; } = string.Empty;
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(ApisInfo), typeof(Apis))]
    public class ApisInfo : Apis
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateApisParam), typeof(Apis))]
    public class UpdateApisParam : Apis
    {
    }
    #endregion

    public class ApisTreeSelectDTO
    {

        public Guid Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public bool Disabled { get; set; }
        public VersionEnum Version { get; set; }
        public List<ApisTreeSelectDTO> Children { get; set; } = [];
    }
}
