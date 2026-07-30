using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.Log
{
    /// <summary>
    /// SQL日志
    /// </summary>
    [LogDataBase]
    [SplitTable(SplitType.Month)]
    [SugarTable($@"{"log_sql"}_{{year}}{{month}}{{day}}", IsDisabledUpdateAll = true)]
    public class AopSqlLog : SerilogBase<long>
    {
    }
}
