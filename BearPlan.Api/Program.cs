using System;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.IdGenerator;
using BearPlan.Core.IdGenerator.Contract;
using BearPlan.Core.Model;
using BearPlan.Common.MultiLanguage.Resources;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Core.Internal;
using BearPlan.Core.Mapping;
using BearPlan.Infrastructure.ActionFilter;
using BearPlan.Infrastructure.Extensions;
using BearPlan.Infrastructure.Middleware;
using BearPlan.Core.Middleware;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.RegisterServices;
using Serilog;

// 强制控制台使用 UTF-8 输出，避免 Block Elements / Emoji 等字符在 GBK 控制台下变成 "?"
Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

//配置雪花ID方法参数
IdHelper.SetIdGeneratorOptions(new IdGeneratorOptions(1));
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
// 配置容器
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        hostingContext.Configuration.ConfigureApplication();
        //config.Sources.Clear();
        config
        //.AddJsonFile(builder.Environment.IsDevelopment() ? "appsettings.Development.json" : "appsettings.json",
        //        optional: true, reloadOnChange: false)
            .AddJsonFile("IpRateLimit.json", optional: true, reloadOnChange: false);
    }).UseSerilogMiddleware()
    .ConfigureContainer<ContainerBuilder>(b => { b.RegisterModule(new AutofacExtensions()); });
builder.ConfigureApplication();


// 配置服务
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IRegister, CustomMapper>();
builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton(new AppSettings(builder.Configuration, builder.Environment));
builder.Services.AddOptionRegisterSetup();
builder.Services.AddCustomMultiLanguagesSetup();
builder.Services.AddSerilogSetup();
builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
builder.Services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
builder.Services.AddCacheSetup();
builder.Services.AddSqlSugarSetup();
builder.Services.AddDbSetup();
builder.Services.AddCorsSetup();
builder.Services.AddMiniProfilerSetup();
builder.Services.AddSwaggerSetup();
builder.Services.AddQuartzNetJobSetup();
builder.Services.AddAuthorizationSetup();
builder.Services.AddBrowserDetection();
builder.Services.AddRedisInitMqSetup();
builder.Services.AddIpStrategyRateLimitSetup();
builder.Services.AddRabbitMqSetup();
builder.Services.AddEventBusSetup();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 设置会话过期时间
    options.Cookie.HttpOnly = true; // 安全设置，防止客户端脚本访问
    options.Cookie.IsEssential = true; // 确保在没有同意 Cookie 的情况下也能使用
});
builder.Services.AddControllers(options =>
    {
        // 异常过滤器
        options.Filters.Add<ExceptionLogFilter>();
        // 审计过滤器
        options.Filters.Add<AuditLogFilter>();
        if (App.GetOptions<SystemOptions>().RunMode == RunMode.Demo)
        {
            //演示模式
            options.Filters.Add<DemoFilter>();
        }
        //     解决控制器入参报错
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    //.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(typeof(BearPlan.Common.MultiLanguage.Resources.Language))
    .AddControllersAsServices()
     .ConfigureApiBehaviorOptions(options =>
     {
         // 全局拦截模型验证失败响应
         options.InvalidModelStateResponseFactory = context =>
         {
             // 组装完整错误信息：字段-错误消息数组
             var errorDic = context.ModelState
                 .Where(kvp => kvp.Value.Errors.Any())
                 .ToDictionary(
                     kvp => kvp.Key,
                     kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                 );

             // 构造统一返回体，附带完整校验错误详情
             var apiResult = ExcutedResult<Dictionary<string, string[]>>.FailedResult(data: errorDic, msg:"参数校验失败");
             // 返回400状态码 + 自定义JSON结构
             return new BadRequestObjectResult(apiResult);
         };
     })
    .AddNewtonsoftJson(options =>
        {
            //全局忽略循环引用
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //options.SerializerSettings.ContractResolver = new CustomContractResolver();
        }
    );


builder.Services.AddIpSearcherSetup();
builder.Services.AddSenparcWeixinServices(builder.Configuration);

// 配置中间件
var app = builder.Build();
var senparcSetting = app.Services.GetRequiredService<IOptions<SenparcSetting>>().Value;
var senparcWeixinSetting = app.Services.GetRequiredService<IOptions<SenparcWeixinSetting>>().Value;
// 注册 CO2NET
var register = RegisterService.Start(senparcSetting).UseSenparcWeixinCacheRedis();

// 注册 Weixin（重点）
register.UseSenparcWeixin(senparcWeixinSetting, senparcSetting, (reg, weixinSetting) =>
{
    // 这里可以继续注册公众号、支付、小程序等
    reg.RegisterMpAccount(senparcWeixinSetting, "又原平台-MP");
});




app.ConfigureApplication();

//实体映射配置
var mapper = app.Services.GetRequiredService<IRegister>();
TypeAdapterConfig.GlobalSettings.Apply(mapper);

//多语言请求扩展
app.UseCustomRequestLocalization();

//获取远程真实ip,如果不是nginx代理部署可以不要
app.UseMiddleware<RealIpMiddleware>();
//处理访问不存在的接口
//app.UseMiddleware<NotFoundMiddleware>();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Use(next => context =>
{
    context.Request.EnableBuffering();
    return next(context);
});

app.UseSession();
// // Swagger Auth
app.UseSwaggerAuthorized();
//Swagger UI
//app.UseSwaggerUiMiddleware(() => Assembly.GetExecutingAssembly().GetManifestResourceStream("BearPlan.Api.index.html"));

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// 业务侧将 Swagger 下拉版本号映射为 VersionEnum 的中文显示名（如 "Web端网站"）
app.UseSwaggerUiMiddleware(provider,
    () => Assembly.GetExecutingAssembly().GetManifestResourceStream("BearPlan.Api.index.html"),
    version => ((VersionEnum)version).GetDisplayName());


//静态文件
app.UseStaticFiles();
// SPA兜底，所有不存在的路径全部转发 index.html
app.MapFallbackToFile("index.html");
//cookie
app.UseCookiePolicy();
//错误页
app.UseStatusCodePages();
// 路由
app.UseRouting();
//IP限流
app.UseIpLimitMiddleware();
// CORS预检放行：提前接管带 Origin 的 OPTIONS 预检（[NotCors] 仅对真实请求生效，预检需此中间件兜底）
app.UseMiddleware<CorsMiddleware>();
// CORS跨域
app.UseCors(App.GetOptions<CorsOptions>().Name);
// 认证
app.UseAuthentication();
// 授权
app.UseAuthorization();
//性能监控
app.UseMiniProfilerMiddleware();

//app.UseHttpMethodOverride();

//种子数据
app.UseDataSeederMiddleware();

// 启动配置面板（放在数据库初始化之后，确保读取到最新的初始化状态）
app.AppConfigNotifier();

//作业调度
app.UseQuartzNetJobMiddleware();

//事件总线配置订阅
app.ConfigureEventBus();

// 注册控制器路由
app.MapControllers();

// 运行
app.Run();
