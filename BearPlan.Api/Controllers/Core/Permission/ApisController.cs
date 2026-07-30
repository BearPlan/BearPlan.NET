using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning;
using BearPlan.Api.Controllers.Common;
using BearPlan.Common.Enums;
using BearPlan.Core.Attributes;
using BearPlan.Core.Extensions;
using BearPlan.Core.Global;
using BearPlan.Core.Helper;
using BearPlan.Core.IdGenerator;
using BearPlan.Core.Pager;
using BearPlan.Entity.Core.Permission;
using BearPlan.IBusiness;
using BearPlan.Models;
using BearPlan.Models.Common;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SqlSugar;


namespace BearPlan.Api.Controllers.Core.Permission;

/// <summary>
/// Api管理
/// </summary>
[Route("/api/[controller]/[action]")]
public class ApisController(IApisService service) : BaseApiController
{
    private readonly IApisService _service = service ?? throw new ArgumentNullException(nameof(service));
    #region CRUD
    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<PagedResults<ApisDTO>> GetPageAsync([FromQuery] ApisParam param) =>
        await _service.GetPageAsync(param);

    /// <summary>
    /// 查询详情
    /// </summary>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<ApisInfo> GetInfoAsync(Guid id) =>
        await _service.GetInfoAsync(id);

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<Guid> AddAsync(UpdateApisParam param) => await _service.AddAsync(param);

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPut]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<Guid> UpdateAsync(UpdateApisParam param) => await _service.UpdateAsync(param);

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete]
    [ApiVersion("1.0", Deprecated = false)]
    public async Task<int> Delete([FromBody] HashSet<Guid> ids) => await _service.DeleteAsync(ids);
    #endregion
    #region 扩展
    /// <summary>
    /// 获取树图
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task<Dictionary<int, List<ApisTreeSelectDTO>>> TreeSelectAsync() => await _service.TreeSelectAsync();






    /// <summary>
    /// 刷新Api列表 只实现了新增的api添加
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task Refresh()
    {
        await _service.DeleteAsync(x => true);

        List<Apis> apis = new List<Apis>();
        foreach (var item in Enum.GetValues(typeof(VersionEnum)))
        {
            VersionEnum version = (VersionEnum)item;

            var types = GlobalType.ApiTypes.Where(x =>
        x.IsClass && typeof(ControllerBase).IsAssignableFrom(x) && x.Name != "TestController" &&
        x.Namespace != "BearPlan.Api.Controllers.Common")
        .OrderBy(x => x.GetCustomAttributes<RouteAttribute>().FirstOrDefault()?.Order).ToList();

            var xmlCommentHelper = new XmlCommentHelper();
            xmlCommentHelper.LoadAll();

            foreach (var type in types)
            {

                var methods = type.GetMethods().Where(m =>
                m.DeclaringType == type)
                .ToList();

                //获取当前路由
                var controllerRouteAttr = type.GetCustomAttribute<RouteAttribute>();
                var controllerRoute = controllerRouteAttr?.Template ?? string.Empty;

                // 从类型名称中提取控制器名称（移除"Controller"后缀）
                var controllerName = type.Name.EndsWith("Controller")
                    ? type.Name.Substring(0, type.Name.Length - 10)
                    : type.Name;
                // 替换控制器名称占位符
                if (controllerRoute.Contains("[controller]"))
                {
                    controllerRoute = controllerRoute.Replace("[controller]", controllerName);
                }
                Console.WriteLine(controllerName);
                //获取XML注释

                var xmlComment = xmlCommentHelper.GetTypeComment(type);


                foreach (var methodInfo in methods)
                {
                    var apiVersions = methodInfo.GetCustomAttributes(typeof(ApiVersionAttribute), true)
                        .OfType<ApiVersionAttribute>()
                        .ToList();

                    var methodAttr = methodInfo.GetCustomAttributes(typeof(HttpMethodAttribute), true)
                        .OfType<HttpMethodAttribute>()
                        .FirstOrDefault();
                    var areaAttr = type.GetCustomAttributes(typeof(AreaAttribute), true);


                    var url = string.Empty;
                    // 替换action占位符
                    if (controllerRoute.Contains("[action]"))
                    {
                        url = controllerRoute.Replace("[action]", methodInfo.Name).Replace("Async", "");
                    }

                    //当前版本包括
                    if (!url.IsNullOrEmpty() && apiVersions.Any(x => x.Versions.Any(v => v.MajorVersion == ((int)version))))
                    {

                        var method = methodAttr?.HttpMethods.FirstOrDefault()?.Trim();

                        Console.WriteLine(url);

                        apis.Add(new Apis()
                        {
                            Id = StringToUuidConverter.GenerateVersion5Uuid(url + version),
                            Group = $"{xmlComment}({controllerName})",
                            Url = url.ToLower(),
                            Description = xmlCommentHelper.GetMethodComment(methodInfo),
                            Method = method,
                            Version = version,
                            IsAudit = !Attribute.IsDefined(methodInfo, typeof(NotAuditAttribute))
                        });
                    }
                }
            }
        }
        await _service.AddAsync(apis);
    }

    /// <summary>
    /// 导出 Common.Enums 命名空间下 *Enum 为 TypeScript 代码（中文注释来源：XML &lt;summary&gt;）
    /// </summary>
    /// <param name="names">按枚举名过滤；为空时返回全部（便于手动调试）</param>
    /// <returns>枚举名 -> TypeScript 代码</returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task<Dictionary<string, string>> GetByNamesEnumsAsync([FromQuery] List<string> names)
    {
        // 加载所有 XML 文档注释，用于补全 /// <summary>
        var xmlCommentHelper = new XmlCommentHelper();
        xmlCommentHelper.LoadAll();

        var selectedTypes = SelectEnumTypes(names);
        var result = new Dictionary<string, string>();
        foreach (var type in selectedTypes)
        {
            result[type.Name] = BuildTypeScriptEnum(type, xmlCommentHelper);
        }

        return await Task.FromResult(result);
    }

    /// <summary>
    /// 导出指定枚举的 [Display(Name)] 显示名映射（枚举名 -> 成员列表）
    /// </summary>
    /// <remarks>
    /// 与 <see cref="GetByNamesEnumsAsync"/> 的区别：本接口以 [Display(Name)] 特性为中文显示名权威源，
    /// 用于前端运行时渲染（如标签文案）；后者导出 TS 类型，注释走 XML &lt;summary&gt;。
    /// Display 缺失时回退为枚举成员名（如 ColumnTypeEnum 的中文标识符天然兼容）。
    /// </remarks>
    /// <param name="names">按枚举名过滤；为空时返回全部（便于手动调试）</param>
    /// <returns>枚举名 -> 成员显示名映射列表</returns>
    [HttpGet]
    [ApiVersion("1.0", Deprecated = false)]
    [AllowAnonymous]
    [NotAudit]
    public async Task<Dictionary<string, List<EnumDisplayItem>>> GetByNamesEnumDisplayAsync([FromQuery] List<string> names)
    {
        var selectedTypes = SelectEnumTypes(names);
        var result = new Dictionary<string, List<EnumDisplayItem>>();
        foreach (var type in selectedTypes)
        {
            var items = type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => new EnumDisplayItem
                {
                    Name = f.Name,
                    Value = Convert.ToInt32(f.GetRawConstantValue(), CultureInfo.InvariantCulture),
                    // 复用 Ext.Enum.GetDisplayName：优先 [Display(Name)]，缺失回退成员名
                    Display = ((Enum)Enum.Parse(type, f.Name)).GetDisplayName()
                })
                .ToList();
            result[type.Name] = items;
        }

        return await Task.FromResult(result);
    }

    /// <summary>
    /// 按 names 过滤 BearPlan.Common.Enums 命名空间下、名称以 Enum 结尾的枚举类型。
    /// names 为空时返回候选全集；非空时按枚举名（忽略大小写）取交集，按名排序。
    /// 供 GetByNamesEnumsAsync / GetByNamesEnumDisplayAsync 共用。
    /// </summary>
    private static List<Type> SelectEnumTypes(List<string> names)
    {
        // 候选集合：仅扫描 BearPlan.Common.Enums 命名空间下、名称以 Enum 结尾的枚举
        var enumTypes = GlobalType.CommonTypes
            .Where(t => t.IsEnum
                        && t.Namespace == "BearPlan.Common.Enums"
                        && t.Name.EndsWith("Enum"))
            .ToList();

        // names 为空：全量返回（手动调试场景）；非空：仅返回交集
        if (names is null || names.Count == 0)
        {
            return enumTypes.OrderBy(t => t.Name).ToList();
        }

        // 命中表：枚举名（忽略大小写）-> 类型
        var nameToType = enumTypes.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
        return names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => nameToType.TryGetValue(n, out var t) ? t : null)
            .Where(t => t is not null)
            .OrderBy(t => t!.Name)
            .ToList()!;
    }

    /// <summary>
    /// 将单个 C# 枚举转换为 TypeScript 代码（保留 /// 风格注释）
    /// </summary>
    private static string BuildTypeScriptEnum(Type enumType, XmlCommentHelper xmlCommentHelper)
    {
        const string fieldIndent = "  ";
        var sb = new StringBuilder();

        // 类型注释
        AppendSummary(sb, xmlCommentHelper.GetTypeComment(enumType), string.Empty);
        sb.AppendLine($"export enum {enumType.Name} {{");

        // 仅取静态公有字段（枚举成员），按声明顺序输出
        var fields = enumType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .ToList();

        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var value = Convert.ToInt32(field.GetRawConstantValue(), global::System.Globalization.CultureInfo.InvariantCulture);

            AppendSummary(sb, xmlCommentHelper.GetFieldOrPropertyComment(field), fieldIndent);

            // 枚举值名称首字母保持原样，TS 标识符大小写敏感，无需转换
            sb.Append($"{fieldIndent}{field.Name} = {value},");

            // 字段之间空一行，保持与示例一致
            if (i < fields.Count - 1)
            {
                sb.AppendLine();
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine();
            }
        }

        sb.Append('}');
        return sb.ToString();
    }

    /// <summary>
    /// 把单段 summary 文本拼成 /// <summary>.../// </summary> 注释块
    /// </summary>
    private static void AppendSummary(StringBuilder sb, string summary, string indent)
    {
        if (string.IsNullOrWhiteSpace(summary))
        {
            return;
        }

        var lines = summary.Replace("\r\n", "\n").Split('\n');
        sb.AppendLine($"{indent}/// <summary>");
        foreach (var line in lines)
        {
            // 保留行间空行：内容为空时仅输出 /// 占位
            sb.AppendLine(string.IsNullOrWhiteSpace(line) ? $"{indent}///" : $"{indent}/// {line.Trim()}");
        }
        sb.AppendLine($"{indent}/// </summary>");
    }
    #endregion
}
