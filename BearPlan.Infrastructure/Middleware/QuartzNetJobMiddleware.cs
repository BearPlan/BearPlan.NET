using BearPlan.Common.MultiLanguage.Resources;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper;
using BearPlan.Core.Helper.Serilog;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.IBusiness;
using BearPlan.TaskService.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BearPlan.Infrastructure.Middleware;

/// <summary>
/// QuartzNet作业调度中间件
/// </summary>
public static class QuartzNetJobMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(QuartzNetJobMiddleware));

    public static void UseQuartzNetJobMiddleware(this IApplicationBuilder app)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));

        try
        {
            if (App.GetOptions<MiddlewareOptions>().QuartzNetJob)
            {
                var quartzNetService = app.ApplicationServices.GetRequiredService<IQuartzNetService>();
                var schedulerCenter = app.ApplicationServices.GetRequiredService<ISchedulerCenterService>();
                var allTaskQuartzList = AsyncHelper.RunSync(() => quartzNetService.QueryAllAsync());
                foreach (var item in allTaskQuartzList)
                {
                    if (!item.Enabled) continue;
                    var results = AsyncHelper.RunSync(() => schedulerCenter.AddScheduleJobAsync(item));
                    if (results)
                    {
                        Logger.Information(
                            $"{Language.Sys_QuartzNet}=>{item.TaskName}=>{Language.Action_StartupSuccess}！");
                    }
                    else
                    {
                        Logger.Error(
                            $"{Language.Sys_QuartzNet}=>{item.TaskName}=>{Language.Action_StartupFailure}！");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error($"Error starting the job scheduling service:\n{e.Message}");
            throw;
        }
    }
}