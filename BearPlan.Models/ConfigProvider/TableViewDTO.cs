using System.ComponentModel.DataAnnotations;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Entity.ConfigProvider;
using BearPlan.Models.Common;
using Newtonsoft.Json;
using SqlSugar;
namespace BearPlan.Models.ConfigProvider
{

    public class TableViewEditParam
    {
        [Required]
        public string Tableof { get; set; } = string.Empty;
        public string Router { get; set; }
        public string ConfigId { get; set; }
    }

    [AutoMapping(typeof(TableViewInfo), typeof(TableView))]
    public class TableViewInfo : RootKeyDTO<long>
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
        /// 字段
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        [JsonIgnore]
        public string ColumnsString { get; set; }

        /// <summary>
        /// 默认排序字段
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        [JsonIgnore]
        public string SortString { get; set; }

        #region 扩展字段
        /// <summary>
        /// 字段
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<TableColumn> Columns
        {
            get { return ColumnsString?.ToObject<List<TableColumn>>() ?? []; }
            set { ColumnsString = value.ToJson(); }
        }
        /// <summary>
        /// 多列排序
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public IDictionary<string, OrderTypeEnum> Sorts
        {
            get { return SortString?.ToObject<IDictionary<string, OrderTypeEnum>>() ?? new Dictionary<string, OrderTypeEnum>(); }
            set { SortString = value.ToJson(); }
        }
        #endregion
    }



    [AutoMapping(typeof(UpdateTableViewParam), typeof(TableView))]
    public class UpdateTableViewParam : RootKeyDTO<long>
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
        /// 字段
        /// </summary>
        public List<TableColumn> Columns { get; set; } = [];

        /// <summary>
        /// 默认排序字段
        /// </summary>
        [Required]
        public IDictionary<string, OrderTypeEnum> Sorts { get; set; } = new Dictionary<string, OrderTypeEnum>();
    }





}
