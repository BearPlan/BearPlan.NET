using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models.Common;

namespace BearPlan.Models.Core.System{
    /// <summary>
    /// Token黑名单
    /// </summary>
    #region 查询参数
    public class TokenBlacklistParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(TokenBlacklistDTO), typeof(TokenBlacklist))]
    public class TokenBlacklistDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 令牌 登录token的MD5值
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(TokenBlacklistInfo), typeof(TokenBlacklist))]
    public class TokenBlacklistInfo : TokenBlacklist
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateTokenBlacklistParam), typeof(TokenBlacklist))]
    public class UpdateTokenBlacklistParam : TokenBlacklist
    {
    }
    #endregion
}
