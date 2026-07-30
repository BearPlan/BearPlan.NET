using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.System;

/// <summary>
/// QuartzNet作业服务
/// </summary>
public class QuartzNetService : BaseServices<QuartzNet>, IQuartzNetService
{
    #region CRUD
    public async Task<PagedResults<QuartzNetDTO>> GetPageAsync(QuartzNetParam param)
    {
        var page = await GetIQueryable().Select(x => new QuartzNetDTO
        {
            Id = x.Id,
            TaskName = x.TaskName,
            TaskGroup = x.TaskGroup,
            Cron = x.Cron,
            AssemblyName = x.AssemblyName,
            ClassName = x.ClassName,
            Description = x.Description,
            Principal = x.Principal,
            AlertEmail = x.AlertEmail,
            PauseAfterFailure = x.PauseAfterFailure,
            RunTimes = x.RunTimes,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            TriggerType = x.TriggerType,
            IntervalSecond = x.IntervalSecond,
            CycleRunTimes = x.CycleRunTimes,
            Enabled = x.Enabled,
            RunParams = x.RunParams,
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    public async Task<QuartzNetInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<QuartzNetInfo>().FirstAsync();
        return entity;
    }

    public async Task<long> AddAsync(UpdateQuartzNetParam param)
    {
        if (await GetIQueryable(q => q.TaskName == param.TaskName).AnyAsync())
        {
            throw new BusException($"TaskName already exists");
        }

        if (await GetIQueryable(q =>
                q.AssemblyName == param.AssemblyName &&
                q.ClassName == param.ClassName).AnyAsync())
        {
            throw new BusException($"ClassName already exists");
        }

        var model = App.Mapper.MapTo<QuartzNet>(param);
        await AddAsync(model);
        return model.Id;
    }

    public async Task<long> UpdateAsync(UpdateQuartzNetParam param)
    {
        var oldQuartzNet = await GetIQueryable(x => x.Id == param.Id).FirstAsync();
        if (oldQuartzNet.IsNull())
        {
            throw new BusException("QuartzNet not found");
        }

        if (oldQuartzNet.TaskName != param.TaskName
            && await GetIQueryable(q => q.TaskName == param.TaskName).AnyAsync())
        {
            throw new BusException($"TaskName already exists");
        }

        if ((oldQuartzNet.AssemblyName != param.AssemblyName ||
             oldQuartzNet.ClassName != param.ClassName) && await GetIQueryable(q =>
                q.AssemblyName == param.AssemblyName &&
                q.ClassName == param.ClassName).AnyAsync())
        {
            throw new BusException($"ClassName already exists");
        }

        var model = App.Mapper.MapTo<QuartzNet>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 更新作业
    /// </summary>
    public async Task UpdateJobInfoAsync(QuartzNet quartzNet)
    {
        await UpdateAsync(quartzNet);
    }

    /// <summary>
    /// 查询全部
    /// </summary>
    public async Task<List<QuartzNet>> QueryAllAsync()
    {
        return await GetIQueryable().ToListAsync();
    }

    /// <summary>
    /// 查询全部作业名称
    /// </summary>
    public async Task<List<QuartzNetDTO>> QueryAllTaskNameAsync()
    {
        return await GetIQueryable().Select<QuartzNetDTO>().ToListAsync();
    }
    #endregion
}
