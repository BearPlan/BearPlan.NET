using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BearPlan.Common.Enums;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.Permission;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// Api路由服务
/// </summary>
public class ApisService : BaseServices<Apis>, IApisService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<ApisDTO>> GetPageAsync(ApisParam param)
    {
        var page = await GetIQueryable().Select(x => new ApisDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<ApisInfo> GetInfoAsync(Guid id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<ApisInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<Guid> AddAsync(UpdateApisParam param)
    {
        var model = App.Mapper.MapTo<Apis>(param);
        await AddAsync(model);
        return model.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    public async Task<Guid> UpdateAsync(UpdateApisParam param)
    {
        var model = App.Mapper.MapTo<Apis>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<int> DeleteAsync(HashSet<Guid> ids)
    {
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 获取树图
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public async Task<Dictionary<int, List<ApisTreeSelectDTO>>> TreeSelectAsync()
    {
        var list = await GetIQueryable().ToListAsync();
        //循环枚举 

        var dic = new Dictionary<int, List<ApisTreeSelectDTO>>();

        foreach (var item in Enum.GetValues(typeof(VersionEnum)))
        {
            VersionEnum version = (VersionEnum)item;
            var tree = list.Where(x => x.Version == version).GroupBy(x => x.Group).Select(x => new ApisTreeSelectDTO()
            {
                Id = Guid.NewGuid(),
                Label = x.Key,
                Version = x.First().Version,
                Children = x.Select(y => new ApisTreeSelectDTO() { Label = y.Description, Id = y.Id, Disabled = !y.IsAudit }).ToList()
            }).ToList();
            dic.Add((int)version, tree);
        }
        return dic;
    }
    #endregion
}
