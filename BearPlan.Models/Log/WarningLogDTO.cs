using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;

namespace BearPlan.Models.Log{
    /// <summary>
    /// 警告日志
    /// </summary>
    #region 查询参数
    public class WarningLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(WarningLogDTO), typeof(WarningLog))]
    public class WarningLogDTO : RootKeyDTO<long>
    {
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(WarningLogInfo), typeof(WarningLog))]
    public class WarningLogInfo : WarningLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateWarningLogParam), typeof(WarningLog))]
    public class UpdateWarningLogParam : WarningLog
    {
    }
    #endregion
}
