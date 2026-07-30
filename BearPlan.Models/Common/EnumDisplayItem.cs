namespace BearPlan.Models.Common
{
    /// <summary>
    /// 枚举成员的显示名映射项，由 ApisController.GetByNamesEnumDisplayAsync 反射 [Display(Name)] 产出。
    /// </summary>
    public class EnumDisplayItem
    {
        /// <summary>
        /// 枚举成员名（C# 标识符），与前端 TS 枚举成员名一致
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 枚举值
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 显示名：优先取 [Display(Name)]；无特性时回退为成员名
        /// </summary>
        public string Display { get; set; } = string.Empty;
    }
}
