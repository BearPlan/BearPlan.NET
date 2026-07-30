using System.ComponentModel;
using System.Diagnostics;
using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper;
using BearPlan.Core.IdGenerator;
using BearPlan.Common.WebApp;
using BearPlan.Core;
using BearPlan.Core.Caches;
using BearPlan.Core.Caches.Redis.MessageQueue;
using BearPlan.Core.ConfigOptions;
using BearPlan.Entity.Log;
using BearPlan.IBusiness;
using IP2Region.Net.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Shyjus.BrowserDetection;

namespace BearPlan.Infrastructure.ActionFilter;

/// <summary>
/// 操作日志过滤器
/// </summary>
public class AuditLogFilter : IAsyncActionFilter
{
    private readonly IAuditLogService _operateLogService;
    private readonly ISettingService _settingService;
    private readonly IBrowserDetector _browserDetector;
    private readonly ISearcher _ipSearcher;

    public AuditLogFilter(IAuditLogService operateLogService, ISearcher searcher,
        ISettingService settingService, IBrowserDetector browserDetector)
    {
        _operateLogService = operateLogService;
        _settingService = settingService;
        _browserDetector = browserDetector;
        _ipSearcher = searcher;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return Execute(context, next);
    }

    /// <summary>
    /// 执行审计功能
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    private async Task Execute(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            var resultContext = await next();
            sw.Stop();

            var action = (ControllerActionDescriptor)context.ActionDescriptor;
          
            if (action.MethodInfo.IsDefined(typeof(NotAuditAttribute), false))
            {
                return;
            }

            //执行结果
            //var action = context.ActionDescriptor as ControllerActionDescriptor;
            //var isTrue = action.MethodInfo.IsDefined(typeof(DescriptionAttribute), false);
            var saveDb = await _settingService.GetSettingValue<bool>("IsAuditLogSaveDB");
            if (saveDb && resultContext.Result.IsNotNull())
            {
                var operateLog = CreateAuditLog(context);
                operateLog.ResponseData = resultContext.Result switch
                {
                    ContentResult contentResult => contentResult.Content,
                    NoContentResult okResult => okResult.ToJson(),
                    OkObjectResult okResult => okResult.Value?.ToJson(),
                    FileContentResult fileContentResult => GetFileContentResult(fileContentResult),
                    ObjectResult objectResult => objectResult.Value?.ToJson(),
                    _ => null // 处理其他未知类型
                };


                //用时
                operateLog.ExecutionDuration = sw.ElapsedMilliseconds;

                if (App.GetOptions<SystemOptions>().UseRedisCache &&
                    App.GetOptions<MiddlewareOptions>().RedisMq)
                {
                    // 实时队列
                    // await App.GetService<ICache>().GetDatabase()
                    //     .ListLeftPushAsync(MqTopicNameKey.AuditLogQueue, operateLog.ToJson());

                    //延迟队列
                    var stopTimeStamp = DateTime.Now.AddSeconds(10).ToUnixTimeStampSecond();
                    await App.GetService<ICache>().GetDatabase()
                        .SortedSetAddAsync(MqTopicNameKey.AuditLogQueue, operateLog.ToJson(), stopTimeStamp);
                }
                else
                {
                    await Task.Factory.StartNew(() => _operateLogService.CreateAsync(operateLog))
                        .ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var ipAddress = _ipSearcher.Search(remoteIp);
            LogHelper.WriteLog(ExceptionHelper.ErrorFormat(context.HttpContext, remoteIp, ipAddress, ex,
                App.GetService<IHttpUser>()?.Account,
                _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                _browserDetector.Browser?.Version), null);
        }
    }

    /// <summary>
    /// 创建审计对象
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private AuditLog CreateAuditLog(ActionExecutingContext context)
    {
        var routeValues = context.ActionDescriptor.RouteValues;

        var httpContext = context.HttpContext;
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var arguments = HttpHelper.GetAllRequestParams(httpContext); //context.ActionArguments;
        var descriptionAttribute = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo
            .GetCustomAttributes(typeof(DescriptionAttribute), true)
            .OfType<DescriptionAttribute>()
            .FirstOrDefault();
        var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
        var descriptionValue = descriptionAttribute != null ? descriptionAttribute.Description : "";

        // 路由值用 TryGetValue 安全取值：不在 Area 下的顶层控制器、健康检查等端点的 RouteValues 中可能没有 "area" key，
        // 直接用索引器访问会抛 KeyNotFoundException，原本的 IsNullOrEmpty 兜底也来不及执行。
        routeValues.TryGetValue("area", out var area);
        routeValues.TryGetValue("controller", out var controller);
        routeValues.TryGetValue("action", out var action);

        var operateLog = new AuditLog
        {
            Id = IdHelper.NextId(),
            CreateBy = App.GetService<IHttpUser>().Account,
            CreateTime = DateTime.Now,
            Area = area.IsNullOrEmpty() ? "" : App.L.R(area),
            Controller = controller,
            Action = action,
            Method = httpContext.Request.Method,
            Description = App.L.R(descriptionValue),
            RequestUrl = httpContext.Request.Path,
            RequestParameters = arguments.ToJson(),
            RequestIp = remoteIp,
            IpAddress = _ipSearcher.Search(remoteIp),
            UserAgent = userAgent,
            OperatingSystem = _browserDetector.Browser?.OS,
            DeviceType = _browserDetector.Browser?.DeviceType,
            BrowserName = _browserDetector.Browser?.Name,
            Version = _browserDetector.Browser?.Version
        };


        var reqUrl = httpContext.Request.Path.Value?.ToLower();
        if (reqUrl is "/auth/login")
        {
            var (_, value) = arguments.SingleOrDefault(k => k.Key == "username");
            if (!value.IsNullOrEmpty())
            {
                operateLog.CreateBy = value.ToString();
            }
        }

        return operateLog;
    }

    private string GetFileContentResult(FileContentResult fileContentResult)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hashBytes = md5.ComputeHash(fileContentResult.FileContents);
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return new
        {
            FileName = fileContentResult.FileDownloadName,
            FileSize = fileContentResult.FileContents.Length,
            ContentType = fileContentResult.ContentType,
            FileHash = hashString
        }.ToJson();
    }
}
