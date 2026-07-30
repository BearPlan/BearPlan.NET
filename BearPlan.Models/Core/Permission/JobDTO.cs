using BearPlan.Core.Attributes;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models.Common;
using BearPlan.Core.Pager;

namespace BearPlan.Models.Core.Permission{
    /// <summary>
    /// 岗位
    /// </summary>
    #region 查询参数
    public class JobParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(JobDTO), typeof(Job))]
    public class JobDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(JobInfo), typeof(Job))]
    public class JobInfo : Job
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateJobParam), typeof(Job))]
    public class UpdateJobParam : Job
    {
    }
    #endregion
}
