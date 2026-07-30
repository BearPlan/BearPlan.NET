using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Common;
using SqlSugar;

namespace BearPlan.Models.Core.System
{
    /// <summary>
    /// 用户微信账号
    /// </summary>
    #region 查询参数
    public class UserWeiXinParam : PageParam
    {
        /// <summary>
        /// 系统用户 Id
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 关键字（OpenId / UnionId / 昵称）
        /// </summary>
        public string Keyword { get; set; }
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(UserWeiXinDTO), typeof(UserWeiXin))]
    public class UserWeiXinDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 系统用户 Id
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 微信开放平台唯一标识
        /// </summary>
        public string OpenId { get; set; } = string.Empty;

        /// <summary>
        /// 微信用户唯一标识（跨应用）
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; } = string.Empty;

        /// <summary>
        /// 微信应用 AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 是否关注公众号：0 表示未关注
        /// </summary>
        public int? Subscribe { get; set; }

        /// <summary>
        /// 最后关注时间（时间戳）
        /// </summary>
        public long? SubscribeTime { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(UserWeiXinInfo), typeof(UserWeiXin))]
    public class UserWeiXinInfo : UserWeiXin
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateUserWeiXinParam), typeof(UserWeiXin))]
    public class UpdateUserWeiXinParam : UserWeiXin
    {
    }
    #endregion
}
