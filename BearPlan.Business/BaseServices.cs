using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BearPlan.Common.MultiLanguage.Resources;
using BearPlan.Core.Extensions;
using BearPlan.Core.Model;
using BearPlan.Core;
using BearPlan.IBusiness;
using BearPlan.Models.Queries.Common;
using BearPlan.Repository.SugarHandler;
using iText.Layout.Element;
using SqlSugar;

namespace BearPlan.Business;

/// <summary>
/// 业务实现基类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class, new()
{
    #region 字段

    /// <summary>
    /// 当前操作对象仓储
    /// </summary>
    public ISugarRepository<TEntity> SugarRepository { get; set; }

    /// <summary>
    /// sugarClient
    /// </summary>
    public ISqlSugarClient SugarClient => SugarRepository.SugarClient;

    #endregion

    #region 构造函数

    // public BaseServices(ISugarRepository<TEntity> sugarRepository)
    // {
    //     if (sugarRepository == null)
    //     {
    //         throw new ArgumentNullException(nameof(sugarRepository), "sugarRepository cannot be null");
    //     }
    //     SugarRepository = sugarRepository;
    // }

    #endregion

    #region 新增

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体集合</param>
    /// <param name="lockString">锁</param>
    /// <returns></returns>
    public async Task<int> AddAsync(TEntity entity, string lockString = "")
    {
        var insert = SugarClient.Insertable(entity);
        if (!lockString.IsNullOrEmpty())
        {
            insert = insert.With(lockString);
        }

        var result = await insert.ExecuteCommandAsync();

        return result;
    }

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entitys">实体集合</param>
    /// <param name="lockString">锁</param>
    /// <returns></returns>
    public async Task<int> AddAsync(List<TEntity> entitys, string lockString = "")
    {
        var insert = SugarClient.Insertable(entitys);
        if (!lockString.IsNullOrEmpty())
        {
            insert = insert.With(lockString);
        }

        var result = await insert.ExecuteCommandAsync();
        return result;
    }

    #endregion

    #region 修改
    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="where">实体对象</param>
    /// <param name="isUpdateColumns">更新列</param>
    /// <param name="lockString">是否加锁</param>
    /// <returns></returns>
    public async Task<int> UpdateAsync(Expression<Func<TEntity, bool>> where,
        Expression<Func<TEntity, TEntity>> isUpdateColumns = null, string lockString = "")
    {


        IUpdateable<TEntity> up = SugarClient.Updateable<TEntity>().SetColumns(isUpdateColumns);
        if (where != null)
        {
            up = up.Where(where);
        }

        if (!lockString.IsNullOrEmpty())
        {
            up = up.With(SqlWith.UpdLock);
        }

        var result = await up.ExecuteCommandAsync();
        return result;
    }


    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="isUpdateColumns">更新列</param>
    /// <param name="ignoreColumns">忽略列</param>
    /// <param name="isIgnoreNull">是否忽略NULL列更新</param>
    /// <param name="lockString">是否加锁</param>
    /// <returns></returns>
    public async Task<int> UpdateAsync(TEntity entity,
        Expression<Func<TEntity, object>> isUpdateColumns = null,
        Expression<Func<TEntity, object>> ignoreColumns = null, bool isIgnoreNull = true, string lockString = "")
    {
        if (isUpdateColumns != null && ignoreColumns != null)
        {
            throw new Exception(Language.Error_UpdateAndExcludeConflict);
        }

        var ignoreIsDeletedFields = (Expression<Func<TEntity, object>>)(x => ((ISoftDeletedEntity)x).IsDeleted);

        ignoreColumns = ignoreColumns == null ? ignoreIsDeletedFields : ignoreColumns.AndAlso(ignoreIsDeletedFields);


        var up = SugarClient.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: isIgnoreNull);
        if (!lockString.IsNullOrEmpty())
        {
            up = up.With(lockString);
        }

        up = up.UpdateColumnsIF(isUpdateColumns != null, isUpdateColumns)
            .IgnoreColumnsIF(ignoreColumns != null, ignoreColumns);

        var result = await up.ExecuteCommandAsync();

        return result ;  
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entitys">实体对象</param>
    /// <param name="isUpdateColumns">更新列</param>
    /// <param name="ignoreColumns">忽略列</param>
    /// <param name="isIgnoreNull">是否忽略NULL列更新</param>
    /// <param name="lockString">是否加锁</param>
    /// <returns></returns>
    public async Task<int> UpdateAsync(List<TEntity> entitys,
        Expression<Func<TEntity, object>> isUpdateColumns = null,
        Expression<Func<TEntity, object>> ignoreColumns = null, bool isIgnoreNull = true, string lockString = "")
    {
        if (isUpdateColumns != null && ignoreColumns != null)
        {
            throw new Exception(Language.Error_UpdateAndExcludeConflict);
        }

        var ignoreIsDeletedFields = (Expression<Func<TEntity, object>>)(x => ((ISoftDeletedEntity)x).IsDeleted);

        ignoreColumns = ignoreColumns == null ? ignoreIsDeletedFields : ignoreColumns.AndAlso(ignoreIsDeletedFields);

        var up = SugarClient.Updateable(entitys).IgnoreColumns(ignoreAllNullColumns: isIgnoreNull);
        if (!lockString.IsNullOrEmpty())
        {
            up = up.With(lockString);
        }

        up = up.UpdateColumnsIF(isUpdateColumns != null, isUpdateColumns)
            .IgnoreColumnsIF(ignoreColumns != null, ignoreColumns);
        var result = await up.ExecuteCommandAsync();
        return result ;
    }

    #endregion

    #region 删除(逻辑删除)

    /// <summary>
    /// 逻辑删除 操作的类需继承ISoftDeletedEntity
    /// </summary>
    /// <param name="exp"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<int> LogicDeleteAsync<T>(Expression<Func<T, bool>> exp) where T : class, ISoftDeletedEntity, new()
    {
        return await SugarClient.Updateable<T>()
            .SetColumns(it => new T { IsDeleted = true },
                true) //true 支持更新数据过滤器赋值字段一起更新
            .Where(exp).ExecuteCommandAsync();
    }

/// <summary>
/// 硬删除
/// </summary>
/// <param name="exp"></param>
/// <returns></returns>
    public async Task<int> DeleteAsync( Expression<Func<TEntity, bool>> exp) => await SugarClient.Deleteable<TEntity>(exp).ExecuteCommandAsync();

    #endregion

    #region Queryable

    /// <summary>
    /// GetIQueryable
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <param name="selectExpression">查询表达式</param>
    /// <param name="orderExpression">排序表达式</param>
    /// <param name="orderByType">排序方式</param>
    /// <param name="isClearCreateByFilter">清除创建人过滤器</param>
    /// <param name="lockString">锁</param>
    /// <param name="cacheDurationInSeconds">缓存时间(秒)</param>
    /// <param name="isSplitTable">是否分表</param>
    /// 
    public ISugarQueryable<TEntity> GetIQueryable(Expression<Func<TEntity, bool>> whereExpression = null,
       Expression<Func<TEntity, TEntity>> selectExpression = null,
       Expression<Func<TEntity, object>> orderExpression = null, OrderByType? orderByType = null,
       bool isClearCreateByFilter = false, string lockString = "", int cacheDurationInSeconds = 0,
       bool isSplitTable = false)
    {
        var table = SugarClient.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).WithCacheIF(cacheDurationInSeconds > 0, cacheDurationInSeconds);

        if (!lockString.IsNullOrEmpty())
        {
            table = table.With(lockString);
        }

        if (isClearCreateByFilter)
        {
            table = table.ClearFilter<ICreateByEntity>();
        }

        if (selectExpression != null)
        {
            table = table.Select(selectExpression);
        }

        if (isSplitTable)
        {
            table = table.SplitTable();
        }

        if (orderExpression != null && orderByType != null)
        {
            table = table.OrderBy(orderExpression, (OrderByType)orderByType);
        }

        return table;
    }
  

    #endregion
}
