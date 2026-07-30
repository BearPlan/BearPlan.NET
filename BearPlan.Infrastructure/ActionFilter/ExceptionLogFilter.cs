using System.ComponentModel;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper;
using BearPlan.Core.IdGenerator;
using BearPlan.Common.WebApp;
using BearPlan.Core.Model;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Entity.Log;
using BearPlan.IBusiness;
using IP2Region.Net.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Shyjus.BrowserDetection;
using StackExchange.Profiling;
using LogLevel = BearPlan.Common.Enums.LogLevel;
using MiniProfiler = StackExchange.Profiling.MiniProfiler;

namespace BearPlan.Infrastructure.ActionFilter;

/// <summary>
/// 异常日志过滤器
/// </summary>
public class ExceptionLogFilter : IAsyncExceptionFilter
{
    private readonly IExceptionLogService _exceptionLogService;
    private readonly ISettingService _settingService;
    private readonly IBrowserDetector _browserDetector;
    private readonly ILogger<ExceptionLogFilter> _logger;
    private readonly ISearcher _ipSearcher;

    public ExceptionLogFilter(IExceptionLogService exceptionLogService, ISearcher searcher,
        ISettingService settingService, IBrowserDetector browserDetector,
        ILogger<ExceptionLogFilter> logger)
    {
        _exceptionLogService = exceptionLogService;
        _settingService = settingService;
        _browserDetector = browserDetector;
        _logger = logger;
        _ipSearcher = searcher;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();
        var statusCode = StatusCodes.Status500InternalServerError;
        var code = StatusCodes.Status500InternalServerError;
        //自定义全局异常
        if (context.Exception is BusException busEx)//业务错误
        {
            code = busEx.ErrorCode;
            statusCode = busEx.HttpStatusCode;
        }


        var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var ipAddress = _ipSearcher.Search(remoteIp);
        string throwMsg = context.Exception.Message; //错误信息
        var actionError = new ActionError { Errors = new Dictionary<string, string>() };
        context.Result = new ContentResult
        {
            Content = ExcutedResult.FailedResult(throwMsg, code).ToJson(),
            ContentType = "application/json; charset=utf-8",
            StatusCode = statusCode
        };
        if (App.GetOptions<MiddlewareOptions>().MiniProfiler)
        {
            MiniProfiler.Current.CustomTiming("Errors：", throwMsg);
        }

        try
        {
            //记录日志
            _logger.LogError(ExceptionHelper.ErrorFormat(context.HttpContext, remoteIp, ipAddress, context.Exception,
                App.GetService<IHttpUser>().Account,
                _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                _browserDetector.Browser?.Version));


            var saveDb = await _settingService.GetSettingValue<bool>("IsExceptionLogSaveDB");
            if (saveDb && exceptionType != typeof(DemoRequestException))
            {
                //记录日志到数据库
                var log = CreateLog(context);
                if (log.IsNotNull())
                {
                    await Task.Factory.StartNew(() => _exceptionLogService.CreateAsync(log))
                        .ConfigureAwait(false);
                }
            }
        }
        catch (Exception e)
        {
            //_logger.LogCritical("LogError出错:" + e.ToString());
            LogHelper.WriteLog(ExceptionHelper.ErrorFormat(context.HttpContext, remoteIp, ipAddress, e,
                App.GetService<IHttpUser>().Account,
                _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                _browserDetector.Browser?.Version), null);
        }
    }

    /// <summary>
    /// 创建异常对象
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private ExceptionLog CreateLog(ExceptionContext context)
    {
        ExceptionLog log = null;
        try
        {
            var routeValues = context.ActionDescriptor.RouteValues;
            var httpContext = context.HttpContext;
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var arguments = HttpHelper.GetAllRequestParams(httpContext);
            var descriptionAttribute = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), true)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault();
            var descriptionValue = descriptionAttribute != null ? descriptionAttribute.Description : "";
            var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            log = new ExceptionLog
            {
                Id = IdHelper.NextId(),
                CreateBy = App.GetService<IHttpUser>().Account,
                CreateTime = DateTime.Now,
                Area = routeValues["area"].IsNullOrEmpty() ? "" : App.L.R(routeValues["area"]),
                Controller = routeValues["controller"],
                Action = routeValues["action"],
                Method = httpContext.Request.Method,
                Description = App.L.R(descriptionValue),
                RequestUrl = httpContext.Request.Path,
                RequestParameters = arguments.ToJson(),
                ExceptionMessage = context.Exception.Message,
                ExceptionMessageFull = Core.Helper.ExceptionHelper.GetExceptionAllMsg(context.Exception),
                ExceptionStack = context.Exception.StackTrace,
                RequestIp = remoteIp,
                IpAddress = _ipSearcher.Search(remoteIp),
                UserAgent = userAgent,
                LogLevel = LogLevel.Error,
                OperatingSystem = _browserDetector.Browser?.OS,
                DeviceType = _browserDetector.Browser?.DeviceType,
                BrowserName = _browserDetector.Browser?.Name,
                Version = _browserDetector.Browser?.Version
            };
        }
        catch
        {
            // ignored
        }

        return log;
    }
}
