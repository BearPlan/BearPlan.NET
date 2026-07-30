using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models.Common;

namespace BearPlan.Models.Log{
    /// <summary>
    /// 信息日志
    /// </summary>
    #region 查询参数
    public class InformationLogParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    ///  分页
    /// </summary>
    [AutoMapping(typeof(InformationLogDTO), typeof(InformationLog))]
    public class InformationLogDTO : RootKeyDTO<long>
    {
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(InformationLogInfo), typeof(InformationLog))]
    public class InformationLogInfo : InformationLog
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateInformationLogParam), typeof(InformationLog))]
    public class UpdateInformationLogParam : InformationLog
    {
    }
    #endregion
}
