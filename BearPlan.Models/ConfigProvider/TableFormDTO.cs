using System.ComponentModel.DataAnnotations;
using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Entity.ConfigProvider;
using BearPlan.Models.Common;

namespace BearPlan.Models.ConfigProvider
{
    public class TableFormEditParam
    {
        [Required]
        public string Tableof { get; set; } = string.Empty;
        public string Router { get; set; }
        public string ConfigId { get; set; }
    }

    [AutoMapping(typeof(TableFormInfo), typeof(TableForm))]
    public class TableFormInfo : RootKeyDTO<long>
    {

        /// <summary>
        /// 配置库
        /// </summary>
        public string ConfigId { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public string Router { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [Required]
        public string Tableof { get; set; } = string.Empty;


        /// <summary>
        /// 多余参数
        /// </summary>
        public string Attrs { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        public string ComponentString { get; set; }

        /// <summary>
        /// 表配置
        /// </summary>
        public string OptionString { get; set; }

        #region 扩展字段
        /// <summary>
        /// 组件
        /// </summary>
        public List<FormComponent> Components
        {
            get { return ComponentString?.ToObject<List<FormComponent>>() ?? []; }
            set { ComponentString = value.ToJson(); }
        }
        #endregion
    }



    [AutoMapping(typeof(UpdateTableFormParam), typeof(TableForm))]
    public class UpdateTableFormParam : RootKeyDTO<long>
    {
        /// <summary>
        /// 配置库
        /// </summary>
        public string ConfigId { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public string Router { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [Required]
        public string Tableof { get; set; } = string.Empty;

        /// <summary>
        /// 多余参数
        /// </summary>
        public string Attrs { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        public List<FormComponent> Components { get; set; } = [];
    }
}
