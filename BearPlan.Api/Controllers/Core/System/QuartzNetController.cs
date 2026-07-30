using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness;
using BearPlan.Models;
using BearPlan.TaskService.Service;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace BearPlan.Api.Controllers.Core.System;

[Route("/api/[controller]/[action]")]
public class QuartzNetController(IQuartzNetService service, ISchedulerCenterService schedulerCenterService)
    : BaseApiController
{
    private readonly IQuartzNetService _service = service ?? throw new ArgumentNullException(nameof(service));
    private readonly ISchedulerCenterService _schedulerCenterService = schedulerCenterService ?? throw new ArgumentNullException(nameof(schedulerCenterService));

    #region CRUD
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<QuartzNetDTO>> GetPageAsync([FromQuery] QuartzNetParam param) =>
        await _service.GetPageAsync(param);

    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<QuartzNetInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateQuartzNetParam param)
    {
        if (param.TriggerType == TriggerType.Cron)
        {
            if (param.Cron.IsNullOrEmpty())
            {
                throw new BusException("Cron is required");
            }

            if (!CronExpression.IsValidExpression(param.Cron))
            {
                throw new BusException("Cron expression is invalid");
            }
        }
        else if (param.TriggerType == TriggerType.Simple)
        {
            if (param.IntervalSecond <= 5)
            {
                throw new BusException("IntervalSecond must be greater than 5");
            }
        }

        var id = await _service.AddAsync(param);
        var quartzNet = await _service.GetInfoAsync(id);
        if (quartzNet != null && quartzNet.Enabled)
        {
            await _schedulerCenterService.AddScheduleJobAsync(App.Mapper.MapTo<QuartzNet>(quartzNet));
        }
        return id;
    }

    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateQuartzNetParam param)
    {
        if (param.TriggerType == TriggerType.Cron)
        {
            if (param.Cron.IsNullOrEmpty())
            {
                throw new BusException("Cron is required");
            }

            if (!CronExpression.IsValidExpression(param.Cron))
            {
                throw new BusException("Cron expression is invalid");
            }
        }
        else if (param.TriggerType == TriggerType.Simple)
        {
            if (param.IntervalSecond < 1)
            {
                throw new BusException("IntervalSecond must be at least 1");
            }
        }

        var id = await _service.UpdateAsync(param);
        var quartzNet = App.Mapper.MapTo<QuartzNet>(param);
        await _schedulerCenterService.DeleteScheduleJobAsync(quartzNet.TaskName, quartzNet.TaskGroup);
        if (quartzNet.Enabled)
        {
            await _schedulerCenterService.AddScheduleJobAsync(quartzNet);
        }
        return id;
    }

    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids)
    {
        var quartzList = await _service.GetIQueryable(x => ids.Contains(x.Id)).ToListAsync();
        var result = await _service.DeleteAsync(ids);
        foreach (var item in quartzList)
        {
            await _schedulerCenterService.DeleteScheduleJobAsync(item.TaskName, item.TaskGroup);
        }
        return result;
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 执行作业
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<bool> ExecuteAsync(long id)
    {
        var quartzNet = await _service.GetIQueryable(x => x.Id == id).FirstAsync();
        if (quartzNet.IsNull()) return false;

        quartzNet.Enabled = true;
        await _service.UpdateJobInfoAsync(quartzNet);
        var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet.TaskName, quartzNet.TaskGroup);
        if (!isTrue)
        {
            return await _schedulerCenterService.AddScheduleJobAsync(quartzNet);
        }
        return false;
    }

    /// <summary>
    /// 暂停作业
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<bool> PauseAsync(long id)
    {
        var quartzNet = await _service.GetIQueryable(x => x.Id == id).FirstAsync();
        if (quartzNet.IsNull()) return false;

        var triggerState = await _schedulerCenterService.GetTriggerStatus(quartzNet.TaskName, quartzNet.TaskGroup);
        if (triggerState == TriggerState.Normal)
        {
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet.TaskName, quartzNet.TaskGroup);
            if (isTrue)
            {
                return await _schedulerCenterService.PauseJob(quartzNet.TaskName, quartzNet.TaskGroup);
            }
        }
        return false;
    }

    /// <summary>
    /// 恢复作业
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<bool> ResumeAsync(long id)
    {
        var quartzNet = await _service.GetIQueryable(x => x.Id == id).FirstAsync();
        if (quartzNet.IsNull()) return false;

        var triggerState = await _schedulerCenterService.GetTriggerStatus(quartzNet.TaskName, quartzNet.TaskGroup);
        if (triggerState == TriggerState.Paused)
        {
            var isTrue = await _schedulerCenterService.IsExistScheduleJobAsync(quartzNet.TaskName, quartzNet.TaskGroup);
            if (isTrue)
            {
                return await _schedulerCenterService.ResumeJob(quartzNet.TaskName, quartzNet.TaskGroup);
            }
        }
        return false;
    }
    #endregion
}
