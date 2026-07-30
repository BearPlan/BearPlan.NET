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
public class EmailMessageTemplateController(IEmailMessageTemplateService service) : BaseApiController
{
    private readonly IEmailMessageTemplateService _service = service ?? throw new ArgumentNullException(nameof(service));

    #region CRUD
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<EmailMessageTemplateDTO>> GetPageAsync([FromQuery] EmailMessageTemplateParam param) =>
        await _service.GetPageAsync(param);

    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<EmailMessageTemplateInfo> GetInfoAsync(long id) =>
        await _service.GetInfoAsync(id);

    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> AddAsync(UpdateEmailMessageTemplateParam param) => await _service.AddAsync(param);

    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<long> UpdateAsync(UpdateEmailMessageTemplateParam param) => await _service.UpdateAsync(param);

    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<long> ids) => await _service.DeleteAsync(ids);
    #endregion
}
