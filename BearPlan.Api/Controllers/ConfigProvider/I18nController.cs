using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Attributes;
using BearPlan.Models.ConfigProvider;
using BearPlan.Core.Pager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.ConfigProvider;

/// <summary>
/// 国际化
/// </summary>
[Route("/api/[controller]/[action]")]
public class I18nController(II18nService service) : BaseApiController
{
    private readonly II18nService _service = service ?? throw new ArgumentNullException(nameof(service));
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [ApiVersion("2.0", Deprecated = false)]
    public async Task<PagedResults<I18nDTO>> GetPageAsync([FromQuery] I18nParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    [ApiVersion("2.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateI18nParam param) => await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    [ApiVersion("2.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateI18nParam param) => await _service.UpdateAsync(param);

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    [ApiVersion("2.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids) => await _service.DeleteAsync(ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 根据语言获取国际化字典
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task<Dictionary<string, string>> GetByLocaleAsync(string locale) =>
        await _service.GetByLocaleAsync(locale);
    #endregion
}
