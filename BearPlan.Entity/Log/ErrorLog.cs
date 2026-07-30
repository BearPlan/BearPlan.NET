using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.Log
{
    /// <summary>
    /// 错误日志
    /// </summary>
    [LogDataBase]
    [SplitTable(SplitType.Month)]
    [SugarTable($@"{"log_error"}_{{year}}{{month}}{{day}}", IsDisabledUpdateAll = true)]
    public class ErrorLog : SerilogBase<long>
    {
    }
}
