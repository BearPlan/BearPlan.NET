using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;
using BearPlan.Entity.Core.Permission;
using BearPlan.Models.Common;
using BearPlan.Core.Pager;
using SqlSugar;

namespace BearPlan.Models.Core.Permission{
    /// <summary>
    /// 系统菜单
    /// </summary>
    #region 查询参数
    public class MenuParam : PageParam
    {
    }
    #endregion

    #region DTO
    /// <summary>
    /// 分页
    /// </summary>
    [AutoMapping(typeof(MenuDTO), typeof(Menu))]
    public class MenuDTO : RootKeyDTO<long>
    {
        /// <summary>
        /// 权限标识符
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Permission { get; set; }

        /// <summary>
        /// 菜单名
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string PathParam { get; set; }

        /// <summary>
        /// 布局
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public LayoutTypeEnum? Layout { get; set; }

        /// <summary>
        /// 重定向
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Redirect { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Component { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ParentId { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public MenuTypeEnum MenuType { get; set; }

        /// <summary>
        /// 图标类型
        /// </summary>
        public IconTypeEnum IconType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 路由标题
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Title { get; set; }

        /// <summary>
        /// 缓存页面
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// 是否免登录
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool Constant { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Icon { get; set; }

        /// <summary>
        /// 本地图标
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string LocalIcon { get; set; }

        /// <summary>
        /// 图标字体大小
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int IconFontSize { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 外链链接
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Href { get; set; }

        /// <summary>
        /// 是否隐藏路由
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool HideInMenu { get; set; }

        /// <summary>
        /// 需要高亮的菜单
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ActiveMenu { get; set; }

        /// <summary>
        /// 是否支持多tab页签
        /// </summary>
        public bool MultiTab { get; set; }

        /// <summary>
        /// 固定制表符顺序
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? FixedIndexInTab { get; set; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    [AutoMapping(typeof(MenuInfo), typeof(Menu))]
    public class MenuInfo : Menu
    {
        public List<MenuQuery> Querys { get; set; } = [];
    }

    /// <summary>
    /// 更新
    /// </summary>
    [AutoMapping(typeof(UpdateMenuParam), typeof(Menu))]
    public class UpdateMenuParam : Menu
    {
        public List<MenuQuery> Querys { get; set; } = [];
    }

    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuTreeDTO
    {

        public long Id { get; set; }
        /// <summary>
        /// 菜单标题
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 路由名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string RouteName { get; set; } = string.Empty;
        public string RoutePath { get; set; } = string.Empty;
        /// <summary>
        /// 父级菜单ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// icon图标
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// 使用本地svg作为icon图标
        /// </summary>
        public string LocalIcon { get; set; } = string.Empty;
        /// <summary>
        /// icon图标
        /// </summary>
        public IconTypeEnum IconType { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MenuTypeEnum MenuType { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool HideInMenu { get; set; }
        /// <summary>
        /// 跳转路由
        /// </summary>
        public string Redirect { get; set; } = string.Empty;
        /// <summary>
        /// 根目录始终显示 
        /// </summary>
        public bool AlwaysShow { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        public List<MenuTreeDTO> Children { get; set; } = [];

    }
    #endregion


    #region Route

    /// <summary>
    /// 菜单Dto
    /// </summary>
    [AutoMapping(typeof(Menu), typeof(RouteDTO))]
    public class RouteDTO : BaseEntityDTO<long>
    {
        public long Id { get; set; }
        /// <summary>
        /// 菜单名
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 重定向
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Redirect { get; set; }


        /// <summary>
        /// 组件
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Component { get; set; }


        /// <summary>
        /// 父级
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ParentId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MenuTypeEnum Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        public RouteMeta Meta { get; set; } = null!;

        public Dictionary<string, string> Props { get; set; } = [];
        /// <summary>
        /// 子节点
        /// </summary>
        public List<RouteDTO> Children { get; set; } = [];
    }
    public class RouteMeta
    {
        /// <summary>
        /// 路由标题(可用来作document.title或者菜单的名称)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Title { get; set; }

        /// <summary>
        /// 权限标识
        /// </summary>
        public List<string> Roles => [];
        /// <summary>
        /// 缓存页面
        /// </summary>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// 当设置为true时，将不会进行登录验证，也不会进行访问路径的权限验证
        /// </summary>
        public bool Constant { get; set; }

        /// <summary>
        /// 菜单和面包屑对应的图标
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// 使用本地svg作为的菜单和面包屑对应的图标(assets/svg-icon文件夹的的svg文件名)
        /// </summary>
        public string LocalIcon { get; set; } = string.Empty;
        /// <summary>
        /// 菜单和面包屑对应的图标的字体大小
        /// </summary>
        public int? IconFontSize { get; set; }


        /// <summary>
        /// 路由顺序，可用于菜单的排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 外链链接
        /// </summary>
        public string Href { get; set; } = string.Empty;
        /// <summary>
        /// 是否在菜单中隐藏路线
        /// </summary>
        public bool HideInMenu { get; set; }


        /// <summary>
        /// 当前路由需要选中的菜单项(用于跳转至不在左侧菜单显示的路由且需要高亮某个菜单的情况)
        /// </summary>
        public string ActiveMenu { get; set; } = string.Empty;


        /// <summary>
        /// 是否支持多个tab页签(默认一个，即相同name的路由会被替换)
        /// </summary>
        public bool MultiTab { get; set; }
        /// <summary>
        /// 如果设置，路线将固定在制表符中，值是固定制表符的顺序
        /// </summary>
        public int? FixedIndexInTab { get; set; }
        /// <summary>
        /// 跳转参数
        /// </summary>
        public List<MenuQuery> Query { get; set; } = [];


    }

    /// <summary>
    /// 参数
    /// </summary>
    public class MenuQuery
    {
        public long Id { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public long? ParentId { get; set; }

        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
    }

    public class MenuButton
    {
        public long Id { get; set; }
        /// <summary>
        /// 按钮编码
        /// </summary>
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; } = string.Empty;
        /// <summary>
        /// 父级Id
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
    }

    public class MenuTreeSelectDTO
    {
        public long Id { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public MenuTypeEnum MenuType { get; set; }
        /// <summary>
        /// icon图标
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// icon图标
        /// </summary>
        public IconTypeEnum IconType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<MenuTreeSelectDTO> Children { get; set; } = [];



    }
    #endregion
}
