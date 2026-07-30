using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;

namespace BearPlan.Models.Log{
    /// <summary>
    /// SQL日志
    /// </summary>
    #region 查询参数
    public class AopSqlLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(AopSqlLogDTO), typeof(AopSqlLog))]
    public class AopSqlLogDTO : RootKeyDTO<long>
    {
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(AopSqlLogInfo), typeof(AopSqlLog))]
    public class AopSqlLogInfo : AopSqlLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateAopSqlLogParam), typeof(AopSqlLog))]
    public class UpdateAopSqlLogParam : AopSqlLog
    {
    }
    #endregion
}
