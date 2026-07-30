using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BearPlan.Infrastructure.ActionFilter;
public class CustomSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        // Microsoft.OpenApi 2.x: IOpenApiSchema 接口上的 Type/Format 为只读，
        // 需要转为具体类 OpenApiSchema 才能赋值；2.x 已移除 Nullable 属性，
        // 可空通过 JsonSchemaType（Flags 枚举）按位或 JsonSchemaType.Null 表达。
        if (schema is not OpenApiSchema concrete)
        {
            return;
        }

        if (context.Type == typeof(long))
        {
            // 将 long 序列化为字符串，避免前端 JS 大整数精度丢失
            concrete.Type = JsonSchemaType.String;
            concrete.Format = null;
        }
        else if (context.Type == typeof(long?))
        {
            // 可空 long：Type 为 String | Null
            concrete.Type = JsonSchemaType.String | JsonSchemaType.Null;
            concrete.Format = null;
        }
    }

}
