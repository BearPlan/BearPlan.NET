using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.Extensions.Logging;
using static BearPlan.Core.Helper.ExceptionHelper;
using BearPlan.Common.Global;

namespace BearPlan.Business.Core.System;

/// <summary>
/// 全局设置服务
/// </summary>
public class SettingService : BaseServices<Setting>, ISettingService
{
    #region CRUD
    public async Task<PagedResults<SettingDTO>> GetPageAsync(SettingParam param)
    {
        var page = await GetIQueryable().Select(x => new SettingDTO
        {
            Id = x.Id,
            Name = x.Name,
            Value = x.Value,
            Enabled = x.Enabled,
            Description = x.Description
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    public async Task<SettingInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<SettingInfo>().FirstAsync();
        return entity;
    }

    public async Task<long> AddAsync(UpdateSettingParam param)
    {
        var model = App.Mapper.MapTo<Setting>(param);
        await AddAsync(model);
        return model.Id;
    }

    public async Task<long> UpdateAsync(UpdateSettingParam param)
    {
        var oldSetting = await GetIQueryable(x => x.Id == param.Id).FirstAsync();
        if (oldSetting != null)
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadSettingByName +
                                        oldSetting.Name.ToMd5String16());
        }
        var model = App.Mapper.MapTo<Setting>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        var settings = await GetIQueryable(x => ids.Contains(x.Id)).ToListAsync();
        foreach (var setting in settings)
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.LoadSettingByName +
                                        setting.Name.ToMd5String16());
        }
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 获取设置值
    /// </summary>
    /// <param name="settingName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [UseCache(Expiration = 30, KeyPrefix = GlobalConstants.CachePrefix.LoadSettingByName)]
    public async Task<T> GetSettingValue<T>(string settingName)
    {
        var setting = await GetIQueryable(x => x.Name == settingName).FirstAsync();

        if (setting == null) return default;

        try
        {
            return (T)ConvertValue(typeof(T), setting.Value);
        }
        catch (Exception e)
        {
            App.GetService<ILogger<Setting>>().LogError(GetExceptionAllMsg(e));
            return default;
        }
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    private static object ConvertValue(Type type, string value)
    {
        if (type == typeof(object))
        {
            return value;
        }

        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return string.IsNullOrEmpty(value) ? value : ConvertValue(Nullable.GetUnderlyingType(type), value);
        }

        var converter = TypeDescriptor.GetConverter(type);
        return converter.CanConvertFrom(typeof(string)) ? converter.ConvertFromInvariantString(value) : null;
    }
    #endregion
}
