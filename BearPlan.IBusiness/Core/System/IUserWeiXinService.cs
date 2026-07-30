using System.Collections.Generic;
using System.Threading.Tasks;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.System;
using BearPlan.Models;

namespace BearPlan.IBusiness.Core.System
{
    /// <summary>
    /// 用户微信账号接口
    /// </summary>
    public interface IUserWeiXinService : IBaseServices<UserWeiXin>
    {
        #region CRUD
        /// <summary>
        /// 分页查询
        /// </summary>
        Task<PagedResults<UserWeiXinDTO>> GetPageAsync(UserWeiXinParam param);

        /// <summary>
        /// 查询详情
        /// </summary>
        Task<UserWeiXinInfo> GetInfoAsync(long id);

        /// <summary>
        /// 新增
        /// </summary>
        Task<long> AddAsync(UpdateUserWeiXinParam param);

        /// <summary>
        /// 编辑
        /// </summary>
        Task<long> UpdateAsync(UpdateUserWeiXinParam param);

        /// <summary>
        /// 删除
        /// </summary>
        Task<int> DeleteAsync(HashSet<long> ids);
        #endregion
    }
}
