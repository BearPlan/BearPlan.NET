using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Attributes;
using BearPlan.Core.Global;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System.Dict;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;
using BearPlan.Common.Global;

namespace BearPlan.Business.Core.System;

/// <summary>
/// 字典服务
/// </summary>
public class DictService : BaseServices<Dict>, IDictService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<DictDTO>> GetPageAsync(DictParam param)
    {
        var page = await GetIQueryable().Select(x => new DictDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<DictInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<DictInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateDictParam param)
    {
        var model = App.Mapper.MapTo<Dict>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateDictParam param)
    {
        var model = App.Mapper.MapTo<Dict>(param);
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
    /// 根据名称查询字典
    /// </summary>
    [UseCache(Expiration = 30, KeyPrefix = GlobalConstants.CachePrefix.LoadDictByName)]
    public async Task<DictInfo> QueryByNameAsync(string name)
    {
        var dict = await GetIQueryable(x => x.Name == name).Select<DictInfo>().FirstAsync();
        return dict;
    }
    #endregion
}
