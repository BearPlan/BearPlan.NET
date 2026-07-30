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
/// 字典详情服务
/// </summary>
public class DictDetailService : BaseServices<DictDetail>, IDictDetailService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<DictDetailDTO>> GetPageAsync(DictDetailParam param)
    {
        var page = await GetIQueryable().Select(x => new DictDetailDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<DictDetailInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<DictDetailInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateDictDetailParam param)
    {
        var model = App.Mapper.MapTo<DictDetail>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateDictDetailParam param)
    {
        var model = App.Mapper.MapTo<DictDetail>(param);
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
    /// 根据字典ID查询详情
    /// </summary>
    [UseCache(Expiration = 30, KeyPrefix = GlobalConstants.CachePrefix.LoadDictDetailByDictId)]
    public async Task<List<DictDetailInfo>> GetDetailByDictIdAsync(long dictId)
    {
        return await GetIQueryable(x => x.DictId == dictId)
            .OrderBy(x => x.DictSort)
            .Select<DictDetailInfo>().ToListAsync();
    }
    #endregion
}
