using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Extensions;
using BearPlan.Core.IdGenerator;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.System;

/// <summary>
/// App应用秘钥
/// </summary>
public class AppSecretService : BaseServices<AppSecret>, IAppSecretService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<AppSecretDTO>> GetPageAsync(AppSecretParam param)
    {
        var page = await GetIQueryable().Select(x => new AppSecretDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<AppSecretInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<AppSecretInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(UpdateAppSecretParam param)
    {
        var model = App.Mapper.MapTo<AppSecret>(param);
        var id = IdHelper.NextId().ToString();
        model.AppId = DateTime.Now.ToString("yyyyMMdd") + id[..8];
        model.AppSecretKey =
            (model.AppId + id).ToHmacsha256String(App.GetOptions<SystemOptions>().HmacSecret);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<long> UpdateAsync(UpdateAppSecretParam param)
    {
        var model = App.Mapper.MapTo<AppSecret>(param);
        await UpdateAsync(model, null, x => new { x.AppId, x.AppSecretKey });
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
