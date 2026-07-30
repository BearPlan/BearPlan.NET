using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BearPlan.Core.Attributes;
using BearPlan.Core.Exception;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Helper;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Core.Utils;
using BearPlan.Entity.Core.Permission;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;
using BearPlan.Common.Global;
using BearPlan.Common.MultiLanguage.Resources;

namespace BearPlan.Business.Core.Permission;

/// <summary>
/// 部门服务
/// </summary>
public class DeptService : BaseServices<Dept>, IDeptService
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    public async Task<PagedResults<DeptDTO>> GetPageAsync(DeptParam param)
    {
        var page = await GetIQueryable().Select(x => new DeptDTO
        {
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    /// <summary>
    /// 查询详情
    /// </summary>
    public async Task<DeptInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<DeptInfo>().FirstAsync();
        return entity;
    }

    /// <summary>
    /// 新增
    /// </summary>
    [UseTran]
    public async Task<long> AddAsync(UpdateDeptParam param)
    {
        if (await GetIQueryable(d => d.Name == param.Name).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param,
                nameof(param.Name)));
        }
        Dept dept =
            App.Mapper.MapTo<Dept>(param);
        await AddAsync(dept);

        //重新计算子节点个数
        if (dept.ParentId != 0)
        {
            var model = await GetIQueryable(x => x.Id == dept.ParentId).FirstAsync();
            if (model.IsNotNull())
            {
                var count = await SugarClient.Queryable<Dept>().Where(x => x.ParentId == dept.Id)
                    .CountAsync();
                model.SubCount = count;

                await UpdateAsync(model);
            }
        }
        return dept.Id;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    [UseTran]
    public async Task<long> UpdateAsync(UpdateDeptParam param)
    {
        var oldUseDept =
          await GetIQueryable(x => x.Id == param.Id).FirstAsync();
        if (oldUseDept.IsNull())
        {
            throw new BusException(ValidationError.NotExist(param,
                nameof(Language.Sys_Dept),
                nameof(param.Id)));
        }

        if (oldUseDept.Name != param.Name &&
            await GetIQueryable(x => x.Name == param.Name).AnyAsync())
        {
            throw new BusException(ValidationError.IsExist(param,
                nameof(param.Name)));
        }

        Dept dept =
            App.Mapper.MapTo<Dept>(param);
        dept.SubCount = oldUseDept.SubCount;
        await UpdateAsync(dept);

        //重新计算子节点个数
        //判断修改前父部门是否与修改后相同  如果相同说明并没有修改上下级部门信息
        if (oldUseDept.ParentId != dept.ParentId)
        {
            if (dept.ParentId != 0)
            {
                var model = await GetIQueryable(x => x.Id == dept.ParentId).FirstAsync();
                if (model.IsNotNull())
                {
                    var count = await SugarClient.Queryable<Dept>().Where(x => x.ParentId == dept.Id)
                        .CountAsync();
                    model.SubCount = count;
                    await UpdateAsync(model, x => x.SubCount);
                }
            }

            if (oldUseDept.ParentId != 0)
            {
                var model =
                    await GetIQueryable(x => x.Id == oldUseDept.ParentId).FirstAsync();
                if (model.IsNotNull())
                {
                    var count = await SugarClient.Queryable<Dept>().Where(x => x.ParentId == dept.Id)
                        .CountAsync();
                    model.SubCount = count;
                    await UpdateAsync(model, x => x.SubCount);
                }
            }
        }
        return dept.Id;
    }

    /// <summary>
    /// 删除
    /// </summary>
    [UseTran]
    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        var deptList = await GetIQueryable(x => ids.Contains(x.ParentId)).Includes(x => x.Users).Includes(x => x.Roles)
                .ToListAsync();
        if (deptList.Count < 1)
        {
            throw new BusException(ValidationError.NotExist());
        }

        if (deptList.Any(dept => dept.Users != null && dept.Users.Count != 0))
        {
            throw new BusException(ValidationError.DataAssociationExists());
        }

        if (deptList.Any(dept => dept.Roles != null && dept.Roles.Count != 0))
        {
            throw new BusException(ValidationError.DataAssociationExists());
        }


        var pIds = deptList.Select(x => x.ParentId);

        var updateDeptList = await GetIQueryable(x => pIds.Contains(x.Id)).ToListAsync();

        var isTrue = await LogicDeleteAsync<Dept>(x => ids.Contains(x.Id));

        if (isTrue > 0)
        {
            if (updateDeptList.Any())
            {
                foreach (var d in updateDeptList)
                {
                    var count = await GetIQueryable(x => x.ParentId == d.Id)
                        .CountAsync();
                    d.SubCount = count;
                }

                isTrue = await UpdateAsync(updateDeptList, x => x.SubCount);
            }
        }


        return isTrue;
    }
    #endregion

    #region 扩展

    /// <summary>
    /// 获取树
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>

    public async Task<List<DeptTreeDTO>> GetTreeAsync(DeptTreeParam param)
    {


        var data = await GetIQueryable().WhereIF(!string.IsNullOrEmpty(param.KeyWord) , x => x.Name.Contains(param.KeyWord)).Select<DeptTreeDTO>().ToTreeAsync(it => it.Children, it => it.ParentId, 0, it => it.Id);

        return data;

    }
    #endregion


}
