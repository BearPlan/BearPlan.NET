namespace BearPlan.Models.ServerInfo;

/// <summary>
/// 服务器资源信息
/// </summary>
public class ServerResourcesInfo
{
    /// <summary>
    /// 运行时间
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// 系统信息
    /// </summary>
    public Sys Sys { get; set; } = null!;

    /// <summary>
    /// Cpu信息
    /// </summary>
    public Cpu Cpu { get; set; } = null!;

    /// <summary>
    /// 内存信息
    /// </summary>
    public Memory Memory { get; set; } = null!;

    /// <summary>
    /// 交换区信息
    /// </summary>
    public Swap Swap { get; set; } = null!;

    /// <summary>
    /// 磁盘信息
    /// </summary>
    public Disk Disk { get; set; } = null!;
}
