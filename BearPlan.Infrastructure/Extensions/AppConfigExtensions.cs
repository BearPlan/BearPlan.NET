using System;
using System.Collections.Generic;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Infrastructure.SeedData;
using Microsoft.AspNetCore.Builder;

namespace BearPlan.Infrastructure.Extensions;

public static class AppConfigExtensions
{
    public static void AppConfigNotifier(this WebApplication app)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));

        var systemOptions = App.GetOptions<SystemOptions>();

        // 监听端口：从 urls 配置解析，兼容 "http://localhost:3000" 等格式
        string port = "3000";
        var urls = app.Configuration["urls"];
        if (!string.IsNullOrEmpty(urls))
        {
            var idx = urls.LastIndexOf(':');
            if (idx >= 0) port = urls[(idx + 1)..];
        }

        #region 基础配置信息（放入左下格）

        var baseSection = new ConsoleHelper.ConfigSection("基础配置信息", new Dictionary<string, string>
        {
            { "配置文件", systemOptions.Env.ToString() },
            { "运行模式", systemOptions.RunMode.ToString() },
            { "初始化数据库", systemOptions.IsInitDb.ToString() },
            { "CORS跨域", systemOptions.IsCqrs.ToString() },
            { "默认密码", systemOptions.UserDefaultPassword },
            { "文件上传限制(M)", systemOptions.FileLimitSize.ToString() },
            { "主库ID", systemOptions.MainDataBase },
            { "日志库ID", systemOptions.LogDataBase },
            { "使用Redis缓存", systemOptions.UseRedisCache.ToString() }
        });

        #endregion 基础配置信息（放入左下格）

        #region 启动状态（放入左下格）

        // 从 SeedService 读取初始化结果（数据库初始化在面板之前完成）
        var dbStatusSection = new ConsoleHelper.ConfigSection("启动状态", new Dictionary<string, string>
        {
            { "监听端口", port },
            { "Master 加载", SeedService.MasterDbReady.ToString() },
            { "Log 加载", SeedService.LogDbReady.ToString() }
        });

        #endregion 启动状态（放入左下格）

        #region Serilog 配置信息（放入右下格）

        var serilogOptions = App.GetOptions<SerilogOptions>();
        var serilogSection = new ConsoleHelper.ConfigSection("Serilog 配置信息", new Dictionary<string, string>
        {
            { "记录SQL日志", serilogOptions.RecordSql.ToString() },
            { "写入到数据库", serilogOptions.ToDb.ToString() },
            { "写入到文件", serilogOptions.ToFile.ToString() },
            { "写入到控制台", serilogOptions.ToConsole.ToString() },
            { "写入到Elasticsearch", serilogOptions.ToElasticsearch.ToString() }
        });

        #endregion Serilog 配置信息（放入右下格）

        #region 中间件配置信息（放入右下格）

        var middlewareOptions = App.GetOptions<MiddlewareOptions>();
        var middlewareSection = new ConsoleHelper.ConfigSection("中间件配置信息", new Dictionary<string, string>
        {
            { "调度作业", middlewareOptions.QuartzNetJob.ToString() },
            { "IP限流", middlewareOptions.IpLimit.ToString() },
            { "性能监控", middlewareOptions.MiniProfiler.ToString() },
            { "Rabbit消息队列", middlewareOptions.RabbitMq.ToString() },
            { "Redis消息队列", middlewareOptions.RedisMq.ToString() },
            { "Elasticsearch", middlewareOptions.Elasticsearch.ToString() }
        });

        #endregion 中间件配置信息（放入右下格）

        #region AOP 配置信息（放入右下格）

        var aopOptions = App.GetOptions<AopOptions>();
        var aopSection = new ConsoleHelper.ConfigSection("AOP 配置信息", new Dictionary<string, string>
        {
            { "事务", aopOptions.Transactions.ToString() },
            { "缓存", aopOptions.Cache.ToString() }
        });

        #endregion AOP 配置信息（放入右下格）

        // 2×2 面板：左上熊 logo | 右上项目信息；左下基础配置+启动状态 | 右下其余配置
        ConsoleHelper.PrintConfigBoard(
            logoLines: BearLogo,
            brandLines: BuildBrandLines(),
            leftSections: new[] { baseSection, dbStatusSection },
            rightSections: new[] { serilogSection, middlewareSection, aopSection });

        Console.WriteLine();
    }

    /// <summary>
    /// 构建右上格项目信息：项目名、技术栈、运行环境、文档/联系方式。
    /// 含 OSC 8 超链接与 emoji。
    /// </summary>
    private static IReadOnlyList<string> BuildBrandLines()
    {
        return new List<string>
        {
            "BearPlan · 轻量级 .NET 后端框架",
            "Swagger + AlovaJS + Vue3 + TS",
            "",
            $"▸ .NET {Environment.Version}    CPU {Environment.ProcessorCount} 核",
            "📘 文档  " + ConsoleHelper.Hyperlink("https://bear.js.org", "https://bear.js.org"),
            "👤 微信  Byte_Xiong"
        };
    }

    /// <summary>
    /// 熊头 logo：用 Block Elements（▟▙▜▛▘▝▖▗▀▄▌▐）字符画绘制，
    /// 圆耳朵 + 单线圆角脸框 + 两个椭圆眼睛。
    /// 行宽统一 20，打印时由 ConsoleHelper 居中显示。
    /// </summary>
    private static readonly IReadOnlyList<string> BearLogo = new[]
    {
        "  ▄▄      ▄▄▖  ",
        "   █▄█▙   ▄▛█▙   ",
        "   ▟▀    ▀▀▀▀    ▙  ",
        "  ▐          ▄   ▐  ",
        "  ▐   ▀▀  ▖  ▀▀  ▐  ",
        "  ▐       ▚▄       ▐  ",
        "  ▜▄▄▄▄▄▄▄▄▄▄▄▄▄▄▛  "
    };
}
