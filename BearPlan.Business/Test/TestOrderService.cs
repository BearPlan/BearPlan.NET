using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Test;
using BearPlan.IBusiness;
using BearPlan.Models.Test;
using SqlSugar;

namespace BearPlan.Business.Test;

/// <summary>
/// 测试订单服务
/// </summary>
public class TestOrderService : BaseServices<TestOrder>, ITestOrderService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<TestOrderDTO>> GetPageAsync(TestOrderParam param)
    {
        var page = await GetIQueryable().Select(x => new TestOrderDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<TestOrderInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<TestOrderInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateTestOrderParam param)
    {
        var model = App.Mapper.MapTo<TestOrder>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateTestOrderParam param)
    {
        var model = App.Mapper.MapTo<TestOrder>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
}
