using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.Core.System
{
    /// <summary>
    /// 用户微信账号
    /// </summary>
    [SugarTable("sys_user_weixin")]
    [SugarIndex("unique_{table}_OpenId", nameof(OpenId), OrderByType.Asc, true)]
    public class UserWeiXin : BaseEntity<long>
    {
        /// <summary>
        /// 系统用户 Id
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public long? UserId { get; set; }

        /// <summary>
        /// 微信开放平台唯一标识
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string OpenId { get; set; } = string.Empty;

        /// <summary>
        /// 微信用户唯一标识（跨应用）
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string UnionId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50)]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnDataType = "Text")]
        public string AvatarUrl { get; set; } = string.Empty;

        /// <summary>
        /// 微信应用 AppId
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string AppId { get; set; }

        /// <summary>
        /// 是否关注公众号：0 表示未关注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? Subscribe { get; set; }

        /// <summary>
        /// 最后关注时间（时间戳）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? SubscribeTime { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Source { get; set; }
    }
}
