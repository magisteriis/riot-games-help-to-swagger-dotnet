using System.Diagnostics;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger;

internal static class OpenApiExtensions
{
    [DebuggerStepThrough]
    public static bool ContainsEnum(this IDictionary<string, OpenApiSchema> schemas, string key)
    {
        return schemas.ContainsKey(key) && (schemas[key].Enum.Any() || (schemas[key].AnyOf.Any() && schemas[key].AnyOf.All(s => s.Enum.Any())));
    }

    [DebuggerStepThrough]
    public static void AddRange(this IList<OpenApiTag> tags, IEnumerable<string> tagNames)
    {
        foreach (var tagName in tagNames.Where(tn => tags.All(t => t.Name != tn)))
            tags.Add(new OpenApiTag {Name = tagName});
    }

    [DebuggerStepThrough]
    public static async Task WriteV3AsJson(this OpenApiDocument document, string path)
    {
        var openApiJson = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
        await File.WriteAllTextAsync(path, openApiJson);
    }
    
    [DebuggerStepThrough]
    public static async Task WriteV3AsYaml(this OpenApiDocument document, string path)
    {
        var openApiYaml = document.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
        await File.WriteAllTextAsync(path, openApiYaml);
    }
}