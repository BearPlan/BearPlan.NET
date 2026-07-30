using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.Log
{
    /// <summary>
    /// 失败日志
    /// </summary>
    [LogDataBase]
    [SplitTable(SplitType.Month)]
    [SugarTable($@"{"log_fatal"}_{{year}}{{month}}{{day}}", IsDisabledUpdateAll = true)]
    public class FatalLog : SerilogBase<long>
    {
    }
}
