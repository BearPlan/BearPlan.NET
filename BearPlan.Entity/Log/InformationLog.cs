using BearPlan.Core.Attributes;
using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.Log
{
    /// <summary>
    /// 信息日志
    /// </summary>
    [LogDataBase]
    [SplitTable(SplitType.Month)]
    [SugarTable($@"{"log_information"}_{{year}}{{month}}{{day}}", IsDisabledUpdateAll = true)]
    public class InformationLog : SerilogBase<long>
    {
    }
}
