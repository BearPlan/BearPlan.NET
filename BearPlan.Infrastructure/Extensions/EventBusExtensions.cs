using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.EventBus;
using BearPlan.EventBus.Abstractions;
using BearPlan.EventBus.EventBusRabbitMQ;
using BearPlan.Infrastructure.Messaging.Rabbit.EventHandling;
using BearPlan.Infrastructure.Messaging.Rabbit.Events;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// 事件总线扩展配置
/// </summary>
public static class EventBusExtensions
{
    public static void AddEventBusSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var eventBusOptions = App.GetOptions<EventBusOptions>();
        if (eventBusOptions.Enabled)
        {
            var subscriptionClientName = eventBusOptions.SubscriptionClientName;

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<UserQueryIntegrationEventHandler>();

            if (App.GetOptions<MiddlewareOptions>().RabbitMq)
            {
                services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
                {
                    var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    var retryCount = App.GetOptions<RabbitOptions>().RetryCount;

                    return new EventBusRabbitMq(sp, rabbitMqPersistentConnection, iLifetimeScope,
                        subscriptionClientName, eventBusSubcriptionsManager, retryCount);
                });
            }
        }
    }


    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBusOptions = App.GetOptions<EventBusOptions>();
        if (eventBusOptions.Enabled)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UserQueryIntegrationEvent, UserQueryIntegrationEventHandler>();
        }
    }
}