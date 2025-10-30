using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using System.Linq;

namespace CreaPrintApi.Swagger;

public class EnumSchemaFilter : ISchemaFilter
{
 public void Apply(OpenApiSchema schema, SchemaFilterContext context)
 {
 if (context.Type.IsEnum)
 {
 schema.Enum = context.Type.GetEnumNames().Select(n => (IOpenApiAny)new OpenApiString(n)).ToList();
 schema.Type = "string";
 }
 }
}
