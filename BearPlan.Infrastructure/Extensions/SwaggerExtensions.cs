using System.Runtime.InteropServices;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using BearPlan.Common.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Helper.Serilog;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Infrastructure.ActionFilter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// swagger扩展配置
/// </summary>
public static class SwaggerExtensions
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(SwaggerExtensions));

    public static void AddSwaggerSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        var basePath = AppContext.BaseDirectory;
        var swaggerOptions = App.GetOptions<SwaggerOptions>();
       

        #region 配置版本管理
        services.AddApiVersioning(option =>
        {
            //版本号以什么形式，什么字段传递
            option.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"));

            // 在不提供版本号时，默认为1.0  如果不添加此配置，不提供版本号时会报错"message": "An API version is required, but was not specified."
            option.AssumeDefaultVersionWhenUnspecified = true;

            // 可选，为true时API返回支持的版本信息
            option.ReportApiVersions = true;
            // 请求中未指定版本时默认为1.0
            option.DefaultApiVersion = new ApiVersion(0, 0);

            //option.ErrorResponses
            //option.ErrorResponses = new MyErrorResponseProvider();
            //默认以当前最高版本进行访问
            //option.ApiVersionSelector = new CurrentImplementationApiVersionSelector(option);
        })
            .AddApiExplorer(opt =>
            {
                //以通知swagger替换控制器路由中的版本并配置api版本
                opt.SubstituteApiVersionInUrl = true;
                // 版本名的格式：v+版本号
                opt.GroupNameFormat = "'v'VVV";
                //是否提供API版本服务
                opt.AssumeDefaultVersionWhenUnspecified = true;

            });

        #endregion


        services.AddSwaggerGen();


        //解决上面报ASP0000警告的方案
        services.AddOptions<SwaggerGenOptions>()
                 .Configure<IApiVersionDescriptionProvider>((options, service) =>
                 {
                     options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                     options.SchemaFilter<CustomSchemaFilter>();//swagger自定义
                                                                // 添加文档信息
                     foreach (var item in service.ApiVersionDescriptions)
                     {

                         options.SwaggerDoc(item.GroupName, CreateInfoForApiVersion(item));
                     }
                     OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
                     {
                         var info = new OpenApiInfo()
                         {
                             //标题
                             // 此处显式调用 BearPlan 自有的 ExtObject.GetDisplayName，避免与
                             // Microsoft.OpenApi 2.x 内置的 EnumExtensions.GetDisplayName 形成二义性。
                             Title = $"{swaggerOptions.Title} {ExtObject.GetDisplayName((VersionEnum)description.ApiVersion.MajorVersion)}",
                             //当前版本
                             Version = description.ApiVersion.ToString(),
                             //文档说明
                             Description = @"",

                             ////联系方式
                             //Contact = new OpenApiContact() { Name = "标题", Email = "", Url = null },
                             ////许可证
                             //License = new OpenApiLicense() { Name = "文档", Url = new Uri("") }
                         };
                         //当有弃用标记时的提示信息
                         if (description.IsDeprecated)
                         {
                             info.Description += " - 此版本已放弃兼容";
                         }
                         return info;
                     }
                     // Swashbuckle 10.x: AddSecurityRequirement 签名改为
                     // Func<OpenApiDocument, OpenApiSecurityRequirement>（按文档生成安全要求），
                     // 因此需要用工厂委托包装。OpenApiSecuritySchemeReference 的 hostDocument
                     // 在工厂回调里可拿到 document，传 null 也能在序列化阶段被解析。
                     options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                         {

                                {
                                    // Microsoft.OpenApi 2.x: OpenApiSecurityRequirement 的键类型由 OpenApiSecurityScheme
                                    // 改为 OpenApiSecuritySchemeReference，旧的 OpenApiReference { Type=..., Id=... }
                                    // 已不再适用。此处使用仅传 referenceId 的构造函数引用 Bearer 方案。
                                    // 2.x 的值类型为 List<string>（不再是 string[]）。
                                    new OpenApiSecuritySchemeReference(referenceId: "Bearer", hostDocument: document),
                                    new List<string>()
                                }
                         });


                     // 开启加权小锁
                     options.OperationFilter<AddResponseHeadersFilter>();
                     options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                     // 在header中添加token，传递到后台
                     options.OperationFilter<SecurityRequirementsOperationFilter>();

                     // JWT认证                                                 
                     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                     {
                         Scheme = "bearer",
                         BearerFormat = "JWT",
                         In = ParameterLocation.Header,
                         Description = "Authorization:Bearer {your JWT token}<br/><b>授权地址:/Test/Login</b>",
                         Name = "Authorization", //jwt默认的参数名称
                         Type = SecuritySchemeType.ApiKey


                     });
                     // 关键：关闭NRT可空推断，不再自动加 ?
                     options.SupportNonNullableReferenceTypes();


                     //给swagger添加过滤器
                     //options.OperationFilter<SwaggerParameterFilter>();
                     // 加载XML注释
                     // 为 Swagger JSON and UI设置xml文档注释路径
                     //获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                     //var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                     var xmls = Directory.GetFiles(basePath, "*.xml");
                     Array.ForEach(xmls, aXml =>
                     {
                         options.IncludeXmlComments(aXml, true);
                     });
                     options.OrderActionsBy(o => o.RelativePath);
                 });

    }

    // public class ExcludeSchemaFilter : ISchemaFilter
    // {
    //     public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    //     {
    //         if (context.Type == typeof(System.Data.DbType))
    //         {
    //             // 将此类型标记为不应生成完整架构
    //             schema.Reference = null;
    //         }
    //     }
    // }
}
