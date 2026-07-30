using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.EventBus.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// rabbitmq扩展配置
/// </summary>
public static class RabbitMqExtensions
{
    public static void AddRabbitMqSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (App.GetOptions<MiddlewareOptions>().RabbitMq)
        {
            var options = App.GetOptions<RabbitOptions>();
            services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = options.Connection,
                    UserName = options.Username,
                    Password = options.Password
                    // RabbitMQ.Client 7.x 移除了 DispatchConsumersAsync 开关：
                    // 7.x 默认就是异步消费，不再需要显式开启。
                };
                var retryCount = options.RetryCount;
                return new RabbitMqPersistentConnection(factory, retryCount);
            });
        }
    }
}