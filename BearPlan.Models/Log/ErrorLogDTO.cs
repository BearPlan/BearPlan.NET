using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;

namespace BearPlan.Models.Log{
    /// <summary>
    /// 错误日志
    /// </summary>
    #region 查询参数
    public class ErrorLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(ErrorLogDTO), typeof(ErrorLog))]
    public class ErrorLogDTO : RootKeyDTO<long>
    {
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(ErrorLogInfo), typeof(ErrorLog))]
    public class ErrorLogInfo : ErrorLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateErrorLogParam), typeof(ErrorLog))]
    public class UpdateErrorLogParam : ErrorLog
    {
    }
    #endregion
}
