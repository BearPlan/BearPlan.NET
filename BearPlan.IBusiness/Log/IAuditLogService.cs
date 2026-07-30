using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Log;
using BearPlan.Models;

namespace BearPlan.IBusiness.Log;

/// <summary>
/// 操作日志接口
/// </summary>
public interface IAuditLogService : IBaseServices<AuditLog>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<AuditLogDTO>> GetPageAsync(AuditLogParam param);
    #endregion
    #region 扩展
    /// <summary>
    /// 创建操作日志（内部方法）
    /// </summary>
    Task CreateAsync(AuditLog operateLog);

    /// <summary>
    /// 批量创建操作日志
    /// </summary>
    Task CreateListAsync(List<AuditLog> operateLogs);

    /// <summary>
    /// 查询当前用户操作日志
    /// </summary>
    Task<PagedResults<AuditLogDTO>> QueryByCurrentAsync(AuditLogParam param);

    /// <summary>
    /// 获取操作日志数量
    /// </summary>
    Task<List<int>> GetOperationNumber(int days = 7);
    #endregion
}
