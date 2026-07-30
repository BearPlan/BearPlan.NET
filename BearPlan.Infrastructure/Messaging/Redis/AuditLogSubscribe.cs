using BearPlan.Core.Attributes.Redis;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper;
using BearPlan.Core.Caches.Redis.MessageQueue;
using BearPlan.Entity.Log;
using BearPlan.IBusiness;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BearPlan.Infrastructure.Messaging.Redis;

public class AuditLogSubscribe : IRedisSubscribe
{
    #region Fields

    private readonly ILogger<AuditLogSubscribe> _logger;
    private readonly IAuditLogService _operateLogService;

    #endregion

    #region Ctor

    public AuditLogSubscribe(IAuditLogService operateLogService, ILogger<AuditLogSubscribe> logger)
    {
        _operateLogService = operateLogService;
        _logger = logger;
    }

    #endregion

    [SubscribeDelay(MqTopicNameKey.AuditLogQueue, true)]
    private async Task DoSub(List<RedisValue> redisValues)
    {
        try
        {
            if (redisValues.Any())
            {
                List<AuditLog> operateLogs = new List<AuditLog>();
                redisValues.ForEach(x => { operateLogs.Add(x.ToString().ToObject<AuditLog>()); });
                await _operateLogService.CreateListAsync(operateLogs);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(ExceptionHelper.GetExceptionAllMsg(e));
        }
    }
}