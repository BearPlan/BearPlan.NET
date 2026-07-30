using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.Core.System
{
    /// <summary>
    /// Token黑名单
    /// </summary>
    [SugarTable("sys_token_blacklist")]
    public class TokenBlacklist : BaseEntity<long>
    {
        /// <summary>
        /// 令牌 登录token的MD5值
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
        
    }
}
