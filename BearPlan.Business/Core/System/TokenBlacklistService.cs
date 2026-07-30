using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.System;

/// <summary>
/// Token黑名单
/// </summary>
public class TokenBlacklistService : BaseServices<TokenBlacklist>, ITokenBlacklistService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<TokenBlacklistDTO>> GetPageAsync(TokenBlacklistParam param)
    {
        var page = await GetIQueryable().Select(x => new TokenBlacklistDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<TokenBlacklistInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<TokenBlacklistInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateTokenBlacklistParam param)
    {
        var model = App.Mapper.MapTo<TokenBlacklist>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateTokenBlacklistParam param)
    {
        var model = App.Mapper.MapTo<TokenBlacklist>(param);
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
}
