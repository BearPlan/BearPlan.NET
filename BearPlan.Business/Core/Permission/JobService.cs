using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.Permission;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 岗位服务
/// </summary>
public class JobService : BaseServices<Job>, IJobService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<JobDTO>> GetPageAsync(JobParam param)
    {
        var page = await GetIQueryable().Select(x => new JobDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<JobInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<JobInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateJobParam param)
    {
        var model = App.Mapper.MapTo<Job>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateJobParam param)
    {
        var model = App.Mapper.MapTo<Job>(param);
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
    #region 扩展
    /// <summary>
    /// 查询所有已启用的岗位
    /// </summary>
    public async Task<List<JobInfo>> QueryAllAsync()
    {
        return await GetIQueryable(x => x.Enabled, null, x => x.Sort, OrderByType.Asc)
            .Select<JobInfo>().ToListAsync();
    }
    #endregion
}
