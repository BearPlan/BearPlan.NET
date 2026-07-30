using RabbitMQ.Client;

namespace BearPlan.EventBus.EventBusRabbitMQ;

/// <summary>
/// RabbitMQ 持久连接
/// </summary>
public interface IRabbitMqPersistentConnection
{
    /// <summary>
    /// 已连接
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 尝试连接
    /// </summary>
    /// <returns></returns>
    Task<bool> TryConnectAsync();

    /// <summary>
    /// 创建 Channel（RabbitMQ.Client 7.x：IModel 已被 IChannel 替代）
    /// </summary>
    /// <returns></returns>
    Task<IChannel> CreateChannelAsync();
}
