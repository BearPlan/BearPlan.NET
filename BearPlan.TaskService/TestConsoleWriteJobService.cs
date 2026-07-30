using System;
using System.Threading.Tasks;
using BearPlan.IBusiness.Core.System;
using BearPlan.IBusiness.Log;
using BearPlan.TaskService.Service;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BearPlan.TaskService;

/// <summary>
/// 输出当前时间到控制台
/// </summary>
public class TestConsoleWriteJobService : JobBase<TestConsoleWriteJobService>, IJob
{
    public TestConsoleWriteJobService(ISchedulerCenterService schedulerCenterService,
        IQuartzNetService quartzNetService, IQuartzNetLogService quartzNetLogService,
        ILogger<TestConsoleWriteJobService> logger)
    {
        QuartzNetService = quartzNetService;
        QuartzNetLogService = quartzNetLogService;
        SchedulerCenterService = schedulerCenterService;
        Logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteJob(context, async () => await Run(context));
    }

    private async Task Run(IJobExecutionContext context)
    {
        await Console.Out.WriteLineAsync("当前时间：" + DateTime.Now + "\n");
        //获取传递参数
        JobDataMap data = context.JobDetail.JobDataMap;
    }
}
