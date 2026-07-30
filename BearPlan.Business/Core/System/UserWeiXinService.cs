using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Core;
using BearPlan.Entity.Core.System;
using BearPlan.Models;

namespace BearPlan.Business.Core.System
{
    /// <summary>
    /// 用户微信账号服务
    /// </summary>
    public class UserWeiXinService : BaseServices<UserWeiXin>, IUserWeiXinService
    {
        #region CRUD
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PagedResults<UserWeiXinDTO>> GetPageAsync(UserWeiXinParam param)
        {
            // 关键字按 OpenId / UnionId / 昵称 模糊匹配
            var keyword = param.Keyword;
            var page = await GetIQueryable(x =>
                    (param.UserId == null || x.UserId == param.UserId) &&
                    (string.IsNullOrEmpty(keyword)
                     || x.OpenId.Contains(keyword)
                     || x.UnionId.Contains(keyword)
                     || x.NickName.Contains(keyword)))
                .Select(x => new UserWeiXinDTO
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    OpenId = x.OpenId,
                    UnionId = x.UnionId,
                    NickName = x.NickName,
                    AvatarUrl = x.AvatarUrl,
                    AppId = x.AppId,
                    Subscribe = x.Subscribe,
                    SubscribeTime = x.SubscribeTime
                }, true)
                .SearchWhere(param)
                .ToPagedResultsAsync(param);
            return page;
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        public async Task<UserWeiXinInfo> GetInfoAsync(long id)
        {
            var entity = await GetIQueryable(x => x.Id == id).Select<UserWeiXinInfo>().FirstAsync();
            return entity;
        }

        /// <summary>
        /// 新增
        /// </summary>
        public async Task<long> AddAsync(UpdateUserWeiXinParam param)
        {
            var model = App.Mapper.MapTo<UserWeiXin>(param);
            await AddAsync(model);
            return model.Id;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        public async Task<long> UpdateAsync(UpdateUserWeiXinParam param)
        {
            var model = App.Mapper.MapTo<UserWeiXin>(param);
            await UpdateAsync(model);
            return param.Id;
        }

        /// <summary>
        /// 删除
        /// </summary>
        public async Task<int> DeleteAsync(HashSet<long> ids)
        {
            return await DeleteAsync(x => ids.Contains(x.Id));
        }
        #endregion
    }
}
