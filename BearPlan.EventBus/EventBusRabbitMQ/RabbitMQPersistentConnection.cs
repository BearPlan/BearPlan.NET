using System.Net.Sockets;
using BearPlan.Core.Helper.Serilog;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace BearPlan.EventBus.EventBusRabbitMQ;

/// <summary>
/// RabbitMQ 持久连接
/// </summary>
public class RabbitMqPersistentConnection : IRabbitMqPersistentConnection
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(RabbitMqPersistentConnection));
    private readonly IConnectionFactory _connectionFactory;
    private readonly int _retryCount;
    IConnection _connection;
    bool _disposed;
    readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1, 1);

    public RabbitMqPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _retryCount = retryCount;
    }

    public bool IsConnected
    {
        get { return _connection != null && _connection.IsOpen && !_disposed; }
    }

    /// <summary>
    /// 创建 Channel（RabbitMQ.Client 7.x：IModel 已被 IChannel 替代，且 CreateModel 改为 CreateChannelAsync）
    /// </summary>
    /// <returns></returns>
    public async Task<IChannel> CreateChannelAsync()
    {
        if (!IsConnected)
        {
            await TryConnectAsync();
        }

        return await _connection.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            if (_connection != null)
            {
                _connection.ConnectionShutdownAsync -= OnConnectionShutdownAsync;
                _connection.CallbackExceptionAsync -= OnCallbackExceptionAsync;
                _connection.ConnectionBlockedAsync -= OnConnectionBlockedAsync;
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
        catch (IOException ex)
        {
            Logger.Fatal(ex.ToString());
        }
        finally
        {
            _syncRoot.Dispose();
        }
    }

    /// <summary>
    /// 尝试连接
    /// </summary>
    /// <returns></returns>
    public async Task<bool> TryConnectAsync()
    {
        Logger.Information("RabbitMQ Client is trying to connect");

        await _syncRoot.WaitAsync();
        try
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        Logger.Warning(
                            $"RabbitMQ Client could not connect after {time.TotalSeconds:n1}s ({ex.Message})");
                    }
                );

            await policy.ExecuteAsync(async () =>
            {
                _connection = await _connectionFactory
                    .CreateConnectionAsync();
            });

            if (IsConnected)
            {
                _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
                _connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
                _connection.ConnectionBlockedAsync += OnConnectionBlockedAsync;

                Logger.Information(
                    $"RabbitMQ Client acquired a persistent connection to '{_connection.Endpoint.HostName}' and is subscribed to failure events");

                return true;
            }

            Logger.Fatal("FATAL ERROR: RabbitMQ connections could not be created and opened");
            return false;
        }
        finally
        {
            _syncRoot.Release();
        }
    }

    /// <summary>
    /// 连接阻塞
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async Task OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;

        Logger.Warning("A RabbitMQ connection is blocked. Trying to re-connect...");
        await TryConnectAsync();
    }

    /// <summary>
    /// 回调异常
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async Task OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;

        Logger.Warning("A RabbitMQ connection throw exception. Trying to re-connect...");
        await TryConnectAsync();
    }

    /// <summary>
    /// 连接关闭
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="reason"></param>
    private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason)
    {
        if (_disposed) return;

        Logger.Warning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
        await TryConnectAsync();
    }
}
