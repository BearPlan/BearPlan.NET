using BearPlan.Core.Extensions;
using BearPlan.Infrastructure.SeedData;
using Microsoft.Extensions.DependencyInjection;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// 数据库上下文扩展配置
/// </summary>
public static class DbExtensions
{
    public static void AddDbSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        services.AddScoped<SeedService>();
        services.AddScoped<DataContext>();
    }
}