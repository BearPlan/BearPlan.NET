using System.Net.Sockets;
using System.Text;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper.Serilog;
using BearPlan.EventBus.Abstractions;
using BearPlan.EventBus.Events;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace BearPlan.EventBus.EventBusRabbitMQ;

/// <summary>
/// RabbitMQ消息队列事件
/// </summary>
public class EventBusRabbitMq : IEventBus, IDisposable, IAsyncDisposable
{
    #region 字段

    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(EventBusRabbitMq));
    const string BrokerName = "bear_event_bus";
    const string AutofacScopeName = "bear_event_bus";
    private readonly IRabbitMqPersistentConnection _persistentConnection;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly int _retryCount;
    private IChannel _consumerChannel;

    private string _subscriptionClientName;

    //后面把AutoFac的改成.net core 自带的生命周期
    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region 构造函数

    public EventBusRabbitMq(IServiceProvider serviceProvider, IRabbitMqPersistentConnection persistentConnection,
        ILifetimeScope autofac, string subscriptionClientName, IEventBusSubscriptionsManager subsManager,
        int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _subscriptionClientName = subscriptionClientName;
        _consumerChannel = CreateConsumerChannelAsync().GetAwaiter().GetResult();
        _autofac = autofac;
        _retryCount = retryCount;
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region 发布与订阅

    /// <summary>
    /// 发布（RabbitMQ.Client 7.x：BasicPublish 改为 BasicPublishAsync，因此方法异步化）
    /// </summary>
    /// <param name="event"></param>
    public async Task PublishAsync(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            await _persistentConnection.TryConnectAsync();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {
                    Logger.Warning(
                        $"Could not publish event: {@event.Id} after {time.TotalSeconds:n1}s ({ex.Message})");
                });
        var eventName = @event.GetType().Name;

        Logger.Information($"Creating RabbitMQ channel to publish event: {@event.Id} ({eventName})");
        await using (var channel = await _persistentConnection.CreateChannelAsync())
        {
            Logger.Information($"Declaring RabbitMQ exchange to publish event: {@event.Id}");
            await channel.ExchangeDeclareAsync(exchange: BrokerName, type: "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            await policy.ExecuteAsync(async () =>
            {
                var properties = new BasicProperties();
                properties.DeliveryMode = DeliveryModes.Persistent; // persistent (RabbitMQ 7.x 改为枚举)

                Logger.Information($"Publishing event to RabbitMQ: {@event.Id}");
                await channel.BasicPublishAsync(
                    exchange: BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }
    }

    /// <summary>
    /// 订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();
        DoInternalSubscriptionAsync(eventName).GetAwaiter().GetResult();

        Logger.Information($"Subscribing to event {eventName} with {typeof(TH).GetGenericTypeName()}");
        _subsManager.AddSubscription<T, TH>();
        StartBasicConsumeAsync().GetAwaiter().GetResult();
    }


    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TH"></typeparam>
    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subsManager.GetEventKey<T>();

        Logger.Information($"Unsubscribing from event {eventName}");
        _subsManager.RemoveSubscription<T, TH>();
    }


    /// <summary>
    /// 动态订阅
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    /// <param name="eventName"></param>
    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        Logger.Information($"Subscribing to dynamic event {eventName} with {typeof(TH).GetGenericTypeName()}");

        DoInternalSubscriptionAsync(eventName).GetAwaiter().GetResult();
        _subsManager.AddDynamicSubscription<TH>(eventName);
        StartBasicConsumeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 取消动态订阅
    /// </summary>
    /// <param name="eventName"></param>
    /// <typeparam name="TH"></typeparam>
    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    #endregion

    #region 订阅事件

    /// <summary>
    /// 订阅管理器删除事件（EventHandler&lt;string&gt; 委托兼容 async void，异常需自行处理）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventName"></param>
    private async void SubsManager_OnEventRemoved(object sender, string eventName)
    {
        if (!_persistentConnection.IsConnected)
        {
            await _persistentConnection.TryConnectAsync();
        }

        await using (var channel = await _persistentConnection.CreateChannelAsync())
        {
            await channel.QueueUnbindAsync(queue: _subscriptionClientName,
                exchange: BrokerName,
                routingKey: eventName);

            if (_subsManager.IsEmpty)
            {
                _subscriptionClientName = string.Empty;
                if (_consumerChannel != null)
                {
                    await _consumerChannel.CloseAsync();
                }
            }
        }
    }


    /// <summary>
    /// 做内部订阅
    /// </summary>
    /// <param name="eventName"></param>
    private async Task DoInternalSubscriptionAsync(string eventName)
    {
        var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
        if (!containsKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync();
            }

            if (_consumerChannel == null)
            {
                _consumerChannel = await CreateConsumerChannelAsync();
            }

            await _consumerChannel.QueueBindAsync(queue: _subscriptionClientName,
                exchange: BrokerName,
                routingKey: eventName);
        }
    }

    #endregion

    #region 消费

    /// <summary>
    /// 开始基本消费
    /// </summary>
    private async Task StartBasicConsumeAsync()
    {
        Logger.Information("Starting RabbitMQ basic consume");
        if (_consumerChannel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.ReceivedAsync += Consumer_ReceivedAsync;

            await _consumerChannel.BasicConsumeAsync(
                queue: _subscriptionClientName,
                autoAck: false,
                consumerTag: "",
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer);
        }
        else
        {
            Logger.Error("StartBasicConsume can't call on _consumerChannel == null");
        }
    }

    /// <summary>
    /// 消费者收到消息（AsyncEventHandler&lt;BasicDeliverEventArgs&gt; 委托）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    /// <returns></returns>
    private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, message);
            //await ProcessEventByNetCore(eventName, message);
        }
        catch (Exception ex)
        {
            Logger.Error($"ERROR Processing message {message} ex:{ex.Message}");
        }

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
        // For more information see: https://www.rabbitmq.com/dlx.html
        await _consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
    }

    #endregion

    #region 创建消费者频道创建消费者频道

    /// <summary>
    /// 创建消费者频道
    /// </summary>
    /// <returns></returns>
    private async Task<IChannel> CreateConsumerChannelAsync()
    {
        if (!_persistentConnection.IsConnected)
        {
            await _persistentConnection.TryConnectAsync();
        }

        Logger.Information("Creating RabbitMQ consumer channel");

        var channel = await _persistentConnection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: BrokerName,
            type: "direct");

        await channel.QueueDeclareAsync(queue: _subscriptionClientName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.CallbackExceptionAsync += async (sender, ea) =>
        {
            Logger.Warning("Recreating RabbitMQ consumer channel");
            if (_consumerChannel != null)
            {
                await _consumerChannel.DisposeAsync();
            }
            _consumerChannel = await CreateConsumerChannelAsync();
            await StartBasicConsumeAsync();
        };

        return channel;
    }

    #endregion

    #region 进程事件

    /// <summary>
    /// 进程事件（使用autofac）推荐
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task ProcessEvent(string eventName, string message)
    {
        Logger.Information($"Processing RabbitMQ event: {eventName}");
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            using (var scope = _autofac.BeginLifetimeScope(AutofacScopeName))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler =
                            scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                        if (handler == null) continue;
                        using dynamic eventData = JObject.Parse(message);
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
        else
        {
            Logger.Warning($"No subscription for RabbitMQ event: {eventName}");
        }
    }

    /// <summary>
    /// 进程事件（使用自带的）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task ProcessEventByNetCore(string eventName, string message)
    {
        Logger.Information($"Processing RabbitMQ event: {eventName}");
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            //安装 Microsoft.Extensions.DependencyInjection扩展包

            using (var scope = _serviceProvider.CreateScope())
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler =
                            scope.ServiceProvider.GetRequiredService(subscription.HandlerType) as
                                IDynamicIntegrationEventHandler;
                        if (handler == null) continue;
                        using dynamic eventData = JObject.Parse(message);
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ServiceProvider.GetRequiredService(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
        else
        {
            Logger.Warning($"No subscription for RabbitMQ event: {eventName}");
        }
    }

    #endregion

    #region 释放

    /// <summary>
    /// 同步释放（向后兼容）
    /// </summary>
    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 异步释放（RabbitMQ.Client 7.x：IChannel/IConnection 转为 IAsyncDisposable）
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_consumerChannel != null)
        {
            await _consumerChannel.DisposeAsync();
        }

        _subsManager.Clear();
        GC.SuppressFinalize(this);
    }

    #endregion
}
