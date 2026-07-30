using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Test;
using BearPlan.Models.Test;

namespace BearPlan.IBusiness.Test;

/// <summary>
/// 测试订单接口
/// </summary>
public interface ITestOrderService : IBaseServices<TestOrder>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<TestOrderDTO>> GetPageAsync(TestOrderParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<TestOrderInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateTestOrderParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateTestOrderParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
}
