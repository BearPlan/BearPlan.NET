using BearPlan.Core.Extensions;
using BearPlan.Core;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.Extensions.DependencyInjection;

namespace BearPlan.Infrastructure.Extensions
{
    /// <summary>
    /// IP地理位置查询扩展
    /// </summary>
    public static class IpSearcherExtensions
    {
        public static void AddIpSearcherSetup(this IServiceCollection services)
        {
            if (services.IsNull()) throw new ArgumentNullException(nameof(services));
            services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content,
                Path.Combine(App.WebHostEnvironment.WebRootPath, "resources", "ip", "ip2region.xdb")));
        }
    }
}