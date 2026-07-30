using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Enums;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.System;

/// <summary>
/// 租户服务
/// </summary>
public class TenantService : BaseServices<Tenant>, ITenantService
{
    #region CRUD
    public async Task<PagedResults<TenantDTO>> GetPageAsync(TenantParam param)
    {
        var page = await GetIQueryable().Select(x => new TenantDTO
        {
            Id = x.Id,
            TenantId = x.TenantId,
            Name = x.Name,
            Description = x.Description,
            TenantType = x.TenantType,
            ConfigId = x.ConfigId,
            DbType = x.DbType,
            ConnectionString = x.ConnectionString,
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    public async Task<TenantInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<TenantInfo>().FirstAsync();
        return entity;
    }

    public async Task<long> AddAsync(UpdateTenantParam param)
    {
        if (await GetIQueryable(r => r.TenantId == param.TenantId).AnyAsync())
        {
            throw new BusException($"TenantId already exists");
        }

        if (param.TenantType == TenantType.Db)
        {
            if (param.DbType.IsNull())
            {
                throw new BusException("DbType is required");
            }

            if (param.ConfigId.IsNullOrEmpty())
            {
                throw new BusException("ConfigId is required");
            }

            if (param.ConnectionString.IsNullOrEmpty())
            {
                throw new BusException("ConnectionString is required");
            }

            if (await GetIQueryable(r => r.ConfigId == param.ConfigId).AnyAsync())
            {
                throw new BusException($"ConfigId already exists");
            }
        }

        var model = App.Mapper.MapTo<Tenant>(param);
        await AddAsync(model);
        return model.Id;
    }

    public async Task<long> UpdateAsync(UpdateTenantParam param)
    {
        var oldTenant = await GetIQueryable(x => x.Id == param.Id).FirstAsync();
        if (oldTenant.IsNull())
        {
            throw new BusException("Tenant not found");
        }

        if (oldTenant.TenantId != param.TenantId &&
            await GetIQueryable(x => x.TenantId == param.TenantId).AnyAsync())
        {
            throw new BusException($"TenantId already exists");
        }

        if (param.TenantType == TenantType.Db)
        {
            if (param.DbType.IsNull())
            {
                throw new BusException("DbType is required");
            }

            if (param.ConfigId.IsNullOrEmpty())
            {
                throw new BusException("ConfigId is required");
            }

            if (param.ConnectionString.IsNullOrEmpty())
            {
                throw new BusException("ConnectionString is required");
            }

            if (oldTenant.ConfigId != param.ConfigId &&
                await GetIQueryable(x => x.ConfigId == param.ConfigId).AnyAsync())
            {
                throw new BusException($"ConfigId already exists");
            }
        }

        var model = App.Mapper.MapTo<Tenant>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        var tenants = await GetIQueryable(x => ids.Contains(x.Id)).Includes(x => x.Users).ToListAsync();
        if (tenants.Exists(x => x.Users != null && x.Users.Count != 0))
        {
            throw new BusException("Cannot delete tenant with associated users");
        }
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 查询全部
    /// </summary>
    public async Task<List<TenantDTO>> QueryAllAsync()
    {
        return await GetIQueryable().Select<TenantDTO>().ToListAsync();
    }
    #endregion
}
