using System.Threading.Tasks;
using BearPlan.IBusiness.Core.Message.Email;
using BearPlan.IBusiness.Core.System;
using BearPlan.IBusiness.Log;
using BearPlan.TaskService.Service;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BearPlan.TaskService;

public class SendEmailJobService : JobBase<SendEmailJobService>, IJob
{
    private readonly IEmailScheduleTask _emailScheduleTask;

    public SendEmailJobService(ISchedulerCenterService schedulerCenterService, IQuartzNetService quartzNetService,
        IQuartzNetLogService quartzNetLogService, IEmailScheduleTask emailScheduleTask,
        ILogger<SendEmailJobService> logger)
    {
        QuartzNetService = quartzNetService;
        QuartzNetLogService = quartzNetLogService;
        _emailScheduleTask = emailScheduleTask;
        SchedulerCenterService = schedulerCenterService;
        Logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteJob(context, async () => await Run(context));
    }

    private async Task Run(IJobExecutionContext context)
    {
        await _emailScheduleTask.ExecuteAsync();
        //获取传递参数
        //JobDataMap data = context.JobDetail.JobDataMap;
    }
}
