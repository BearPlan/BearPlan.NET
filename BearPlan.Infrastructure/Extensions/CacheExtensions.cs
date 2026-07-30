using BearPlan.Core.Extensions;
using BearPlan.Core;
using BearPlan.Core.Caches;
using BearPlan.Core.Caches.Distributed;
using BearPlan.Core.Caches.Redis;
using BearPlan.Core.ConfigOptions;
using Microsoft.Extensions.DependencyInjection;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// 缓存扩展配置
/// </summary>
public static class CacheExtensions
{
    public static void AddCacheSetup(this IServiceCollection services)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        services.AddDistributedMemoryCache(); //session需要

        if (App.GetOptions<SystemOptions>().UseRedisCache)
        {
            services.AddSingleton<ICache, RedisCache>();
            // Redis Pub/Sub 订阅能力，用于跨实例事件通知（如扫码登录 SSE 推送）
            services.AddSingleton<IRedisSubscriber, RedisSubscriber>();
            return;
        }

        services.AddSingleton<ICache, DistributedCache>();
    }
}