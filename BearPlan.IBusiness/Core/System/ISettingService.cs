using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.System;

/// <summary>
/// 全局设置接口
/// </summary>
public interface ISettingService : IBaseServices<Setting>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<SettingDTO>> GetPageAsync(SettingParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<SettingInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateSettingParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateSettingParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 根据名称查询设置值
    /// </summary>
    Task<T> GetSettingValue<T>(string settingName);
    #endregion
}
