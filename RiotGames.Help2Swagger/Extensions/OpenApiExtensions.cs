using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger.Extensions;

internal static class OpenApiExtensions
{
    [DebuggerStepThrough]
    public static bool ContainsEnum(this IDictionary<string, OpenApiSchema> schemas, string key)
    {
        return schemas.ContainsKey(key) && (schemas[key].Enum.Any() || schemas[key].AnyOf.All(s => s.Enum.Any()));
    }

    [DebuggerStepThrough]
    public static void AddRange(this IList<OpenApiTag> tags, IEnumerable<string> tagNames)
    {
        foreach (var tagName in tagNames.Where(tn => tags.All(t => t.Name != tn)))
            tags.Add(new OpenApiTag {Name = tagName});
    }

    [DebuggerStepThrough]
    public static void AddNoContent(this OpenApiResponses responses)
    {
        responses.Add("204", new OpenApiResponse
        {
            Description = "No content"
        });
    }
}