using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Core.Pager;
using BearPlan.IBusiness;
using BearPlan.Models;
using Microsoft.AspNetCore.Mvc;

namespace BearPlan.Api.Controllers.Core.Message.Email;

[Route("/api/[controller]/[action]")]
public class EmailAccountController(IEmailAccountService service) : BaseApiController
{
    private readonly IEmailAccountService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<EmailAccountDTO>> GetPageAsync([FromQuery] EmailAccountParam param) =>
        await _service.GetPageAsync(param);

    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<EmailAccountInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateEmailAccountParam param) => await _service.AddAsync(param);

    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateEmailAccountParam param) => await _service.UpdateAsync(param);

    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids) => await _service.DeleteAsync(ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 获取所有邮箱账户
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<List<EmailAccountDTO>> QueryAllAsync() => await _service.QueryAllAsync();
    #endregion
}
