using BearPlan.Core.Attributes;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models.Common;
using Newtonsoft.Json;
using SqlSugar;

namespace BearPlan.Models.Core.Permission{
    /// <summary>
    /// 部门
    /// </summary>
    #region 查询参数
    public class DeptParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(DeptDTO), typeof(Dept))]
    public class DeptDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 父级部门ID
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 子节点个数
        /// </summary>
        public int SubCount { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(DeptInfo), typeof(Dept))]
    public class DeptInfo : Dept
    {
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateDeptParam), typeof(Dept))]
    public class UpdateDeptParam : Dept
    {
    }
    #endregion

    #region 扩展


    public class DeptTreeParam
    {
        public string KeyWord { get; set; }

    }

    [AutoMapping(typeof(Dept), typeof(UpdateDeptParam))]
    public class DeptTreeDTO : BaseEntityDTO<long>
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 父级部门ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DeptTreeDTO> Children { get; set; } = [];
    }

    #endregion


}
