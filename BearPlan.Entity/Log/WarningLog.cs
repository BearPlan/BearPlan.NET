using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.Log
{
    /// <summary>
    /// 警告日志
    /// </summary>
    [LogDataBase]
    [SplitTable(SplitType.Month)]
    [SugarTable($@"{"log_warning"}_{{year}}{{month}}{{day}}", IsDisabledUpdateAll = true)]
    public class WarningLog : SerilogBase<long>
    {
    }
}
