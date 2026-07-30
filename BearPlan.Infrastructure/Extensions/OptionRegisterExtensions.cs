using System.Reflection;
using BearPlan.Core.Attributes;
using BearPlan.Core.Global;
using BearPlan.Core.ConfigOptions.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BearPlan.Infrastructure.Extensions;

/// <summary>
/// 可配置选项扩展
/// </summary>
public static class OptionRegisterExtensions
{
    /// <summary>
    /// 注册配置选项
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddOptionRegisterSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var optionTypes = GlobalType.CoreTypes
            .Where(x => x.GetCustomAttribute<OptionsSettingsAttribute>() != null).ToList();

        foreach (var optionType in optionTypes)
        {
            services.AddConfigurableOptions(optionType);
        }
    }
}