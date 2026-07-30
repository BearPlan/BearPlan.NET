using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using Microsoft.Extensions.DependencyInjection;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// 跨域扩展配置
/// </summary>
public static class CorsExtensions
{
    public static void AddCorsSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        var options = App.GetOptions<CorsOptions>();
        services.AddCors(c =>
        {
            // 标记了 [NotCors] 的接口走此策略：允许任意来源/方法/请求头，不受全局策略限制
            c.AddPolicy(NotCorsAttribute.AllowAllPolicy, policy => policy
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader());

            if (options.EnableAll)
            {
                //允许任意跨域请求
                c.AddPolicy(options.Name,
                    policy =>
                    {
                        policy
                            .SetIsOriginAllowed(host => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            }
            else
            {
                c.AddPolicy(options.Name,
                    policy =>
                    {
                        policy
                            .WithOrigins(options.Policy.Select(x => x.Domain).ToArray())
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            }
        });
    }
}