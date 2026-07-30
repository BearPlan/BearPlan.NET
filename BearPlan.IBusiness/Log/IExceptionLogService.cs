using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models;

namespace BearPlan.IBusiness.Log;

/// <summary>
/// 系统日志接口
/// </summary>
public interface IExceptionLogService : IBaseServices<ExceptionLog>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<ExceptionLogDTO>> GetPageAsync(ExceptionLogParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<ExceptionLogInfo> GetInfoAsync(long id);
    #endregion
    #region 扩展
    /// <summary>
    /// 创建异常日志（内部方法）
    /// </summary>
    Task CreateAsync(ExceptionLog exceptionLog);

    /// <summary>
    /// 获取异常日志数量
    /// </summary>
    Task<List<int>> GetOperationNumber(int days = 7);
    #endregion
}
