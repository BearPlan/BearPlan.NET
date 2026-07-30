using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.Message.Email;
using BearPlan.IBusiness;
using BearPlan.Models;
using SqlSugar;

namespace BearPlan.Business.Core.Message.Email;

/// <summary>
/// 邮箱账户服务
/// </summary>
public class EmailAccountService : BaseServices<EmailAccount>, IEmailAccountService
{
    #region CRUD
    public async Task<PagedResults<EmailAccountDTO>> GetPageAsync(EmailAccountParam param)
    {
        var page = await GetIQueryable().Select(x => new EmailAccountDTO
        {
            Id = x.Id,
            Email = x.Email,
            DisplayName = x.DisplayName,
            Host = x.Host,
            Port = x.Port,
            UserName = x.UserName,
            EnableSsl = x.EnableSsl,
            UseDefaultCredentials = x.UseDefaultCredentials,
        }, true).SearchWhere(param).ToPagedResultsAsync(param);
        return page;
    }

    public async Task<EmailAccountInfo> GetInfoAsync(long id)
    {
        var entity = await GetIQueryable(x => x.Id == id).Select<EmailAccountInfo>().FirstAsync();
        return entity;
    }

    public async Task<long> AddAsync(UpdateEmailAccountParam param)
    {
        var model = App.Mapper.MapTo<EmailAccount>(param);
        await AddAsync(model);
        return model.Id;
    }

    public async Task<long> UpdateAsync(UpdateEmailAccountParam param)
    {
        var model = App.Mapper.MapTo<EmailAccount>(param);
        await UpdateAsync(model);
        return param.Id;
    }

    public async Task<int> DeleteAsync(HashSet<long> ids)
    {
        return await DeleteAsync(x => ids.Contains(x.Id));
    }
    #endregion
    #region 扩展
    /// <summary>
    /// 查询所有
    /// </summary>
    public async Task<List<EmailAccountDTO>> QueryAllAsync()
    {
        return await GetIQueryable().Select<EmailAccountDTO>().ToListAsync();
    }
    #endregion
}
