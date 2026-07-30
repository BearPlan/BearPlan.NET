using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.ConfigProvider;
using BearPlan.Models.ConfigProvider;

namespace BearPlan.IBusiness.ConfigProvider;

/// <summary>
/// 国际化接口
/// </summary>
public interface II18nService : IBaseServices<I18n>
{
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResults<I18nDTO>> GetPageAsync(I18nParam param);

    /// <summary>
    /// 查询详情
    /// </summary>
    Task<I18nInfo> GetInfoAsync(long id);

    /// <summary>
    /// 新增
    /// </summary>
    Task<long> AddAsync(UpdateI18nParam param);

    /// <summary>
    /// 编辑
    /// </summary>
    Task<long> UpdateAsync(UpdateI18nParam param);

    /// <summary>
    /// 删除
    /// </summary>
    Task<int> DeleteAsync(HashSet<long> ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 根据语言获取国际化字典
    /// </summary>
    Task<Dictionary<string, string>> GetByLocaleAsync(string locale);
    #endregion
}
