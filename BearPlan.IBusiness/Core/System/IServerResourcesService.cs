using BearPlan.Models.ServerInfo;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// 服务器资源信息接口
/// </summary>
public interface IServerResourcesService
{
    /// <summary>
    /// 查询
    /// </summary>
    /// <returns></returns>
    Task<ServerResourcesInfo> GetInfoAsync();
}
