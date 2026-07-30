using System.IO;
using System.Linq;
using System.Reflection;
using BearPlan.Common.WebApp;
using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Helper;
using BearPlan.Entity.Core.System;
using BearPlan.Repository.UnitOfWork;
using SqlSugar;

namespace BearPlan.Repository.SugarHandler;

/// <summary>
/// SqlSugar仓储
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SugarRepository<TEntity> : ISugarRepository<TEntity> where TEntity : class, new()
{
    public SugarRepository(IUnitOfWork unitOfWork, IHttpUser httpUser)
    {
        var sqlSugarScope = unitOfWork.GetDbClient();
        var logDbAttribute = typeof(TEntity).GetCustomAttribute<LogDataBaseAttribute>();
        if (logDbAttribute != null)
        {
            SugarClient = sqlSugarScope.GetConnectionScope(AppSettings.GetValue<string>("System", "LogDataBase"));
            return;
        }

        var configProviderDbAttribute = typeof(TEntity).GetCustomAttribute<ConfigProviderDataBaseAttribute>();
        if (configProviderDbAttribute != null)
        {
            SugarClient = sqlSugarScope.GetConnectionScope(AppSettings.GetValue<string>("System", "ConfigProviderDataBase"));
            return;
        }

        //使用多租户
        var useMultiTenant = AppSettings.GetValue<bool>("Tenant", "Enabled");
        if (useMultiTenant)
        {
            var multiDbTenantAttribute = typeof(TEntity).GetCustomAttribute<MultiDbTenantAttribute>();
            if (multiDbTenantAttribute != null)
            {
                if (httpUser.IsNotNull() && httpUser.TenantId > 0)
                {
                    var tenants = sqlSugarScope.Queryable<Tenant>().WithCache(86400).ToList();
                    var tenant = tenants.FirstOrDefault(x => x.TenantId == httpUser.TenantId);
                    if (tenant != null)
                    {
                        var iTenant = sqlSugarScope.AsTenant();
                        if (!iTenant.IsAnyConnection(tenant.ConfigId))
                        {
                            var connectionString = tenant.ConnectionString;
                            if (tenant.DbType == DbType.Sqlite)
                            {
                                connectionString = "DataSource=" +
                                                   Path.Combine(AppSettings.ContentRootPath,
                                                       tenant.ConnectionString);
                            }

                            iTenant.AddConnection(TenantHelper.GetConnectionConfig(tenant.ConfigId, tenant.DbType.Value,
                                connectionString));
                        }

                        SugarClient = iTenant.GetConnectionScope(tenant.ConfigId);
                        return;
                    }
                }
            }
        }

        SugarClient = sqlSugarScope;
    }

    public ISqlSugarClient SugarClient { get; }
}
