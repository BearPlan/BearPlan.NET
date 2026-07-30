using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;

namespace BearPlan.Models.Log{
    /// <summary>
    /// 失败日志
    /// </summary>
    #region 查询参数
    public class FatalLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(FatalLogDTO), typeof(FatalLog))]
    public class FatalLogDTO : RootKeyDTO<long>
    {
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(FatalLogInfo), typeof(FatalLog))]
    public class FatalLogInfo : FatalLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateFatalLogParam), typeof(FatalLog))]
    public class UpdateFatalLogParam : FatalLog
    {
    }
    #endregion
}
