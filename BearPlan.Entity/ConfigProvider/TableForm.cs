using System;
using System.Collections.Generic;
using BearPlan.Core.Attributes;
using BearPlan.Common.Enums;
using BearPlan.Core.Enums;
using BearPlan.Core.Extensions;
using BearPlan.Core.Entity;
using SqlSugar;

namespace BearPlan.Entity.ConfigProvider
{
    /// <summary>
    /// 表格重写
    /// </summary>
    [SugarTable("table_form")]
    [ConfigProviderDataBase]
    //[Tenant(AppConfig.TenantTable)]

    public class TableForm : BaseEntity<long>
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Tableof { get; set; } = string.Empty;
        /// <summary>
        /// 路由
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Router { get; set; }



        /// <summary>
        /// 多余参数 
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string Attrs { get; set; }

        /// <summary>
        /// 多余参数 
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ConfigId { get; set; }





        /// <summary>
        /// 组件
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string ComponentString { get; set; }

        /// <summary>
        /// 表配置
        /// </summary>
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string OptionString { get; set; }



        #region 导航

        /// <summary>
        /// 组件
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<FormComponent> Components {
            get { return ComponentString?.ToObject<List<FormComponent>>() ?? []; }
            set { ComponentString = value.ToJson(); }

        }
       
        #endregion
    }


    /// <summary>
    /// 表格重写
    /// </summary>
    public class FormComponent
    {

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Label { get; set; } = string.Empty;
        /// <summary>
        /// 字段
        /// </summary>
        public string Prop { get; set; } = string.Empty;

        /// <summary>
        /// 组件类型
        /// </summary>
        public ColumnTypeEnum Type { get; set; } = ColumnTypeEnum.自定义;
        
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required{ get; set; }
        
        /// <summary>
        /// 是否自定义
        /// </summary>
        public bool IsCustom { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public Int32 Sort { get; set; }
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsEditDel { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 插槽
        /// </summary>
        public Dictionary<string, string> Slots { get; set; } = [];
        
        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<string, string> Attrs { get; set; } = [];
        
        

        public List<FormMetadata> Metadata { get; set; } = [];
 

       
        public List<FormComponent> Children { get; set; } = [];
    }


    public class FormMetadata
    {
        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, string> Attributes {  get; set; } = [];
        
        /// <summary>
        /// 插槽
        /// </summary>
        public Dictionary<string, string> Slots { get; set; } = [];

        /// <summary>
        /// 事件
        /// </summary>
        public Dictionary<string, string> Events { get; set; } = [];
        /// <summary>
        ///   组件支持的方法
        /// </summary>
        public Dictionary<string, string> Methods { get; set; } = [];
    }



    /// <summary>
    /// 表字段重写
    /// </summary>
    public class FromColumn
    {

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Label { get; set; } = string.Empty;
        /// <summary>
        /// 字段
        /// </summary>
        public string Prop { get; set; } = string.Empty;
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsEditDel { get; set; }
        /// <summary>
        /// 多余参数 
        /// </summary>
        public string Attrs { get; set; } = string.Empty;
    }


}
