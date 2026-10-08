using Newtonsoft.Json;

namespace BearPlan.Models.Common
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class BaseEntityDTO<T> where T : IEquatable<T>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public T Id { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public long? CreateId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public long? UpdateId { get; set; }


        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
