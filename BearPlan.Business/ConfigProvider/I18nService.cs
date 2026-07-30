using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Exception;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.ConfigProvider;
using BearPlan.Models.ConfigProvider;
using SqlSugar;

namespace BearPlan.Business.ConfigProvider;

/// <summary>
/// 国际化服务
/// </summary>
public class I18nService : BaseServices<I18n>, II18nService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<I18nDTO>> GetPageAsync(I18nParam param)
    {
        var page = await GetIQueryable().Select(x => new I18nDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<I18nInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<I18nInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateI18nParam param)
    {
        var model = App.Mapper.MapTo<I18n>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateI18nParam param)
    {
        var model = App.Mapper.MapTo<I18n>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 根据语言获取国际化字典
    /// </summary>
    public async Task<Dictionary<string, string>> GetByLocaleAsync(string locale)
    {
        locale = locale.Replace("-", "_");
        var allowedColumns = new HashSet<string> { "zh_CN", "en_Us" };
        if (!allowedColumns.Contains(locale))
        {
            throw new BusException($"Unsupported locale: {locale}");
        }

        var sql = $"SELECT `key` , `{locale}` as `val` FROM `i18n`";
        var entity = await SugarClient.SqlQueryable<dynamic>(sql).ToListAsync();

        Dictionary<string, string> dic = new Dictionary<string, string>();
        entity.ForEach(x => dic.Add(x.key, x.val));
        return dic;
    }
    #endregion
}
