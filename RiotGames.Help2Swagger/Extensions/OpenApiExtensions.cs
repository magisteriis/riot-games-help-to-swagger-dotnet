using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger;

internal static class OpenApiExtensions
{
    [DebuggerStepThrough]
    public static bool ContainsEnum(this IDictionary<string, OpenApiSchema> schemas, string key)
    {
        return schemas.ContainsKey(key) && (schemas[key].Enum.Any() ||
                                            schemas[key].AnyOf.Any() && schemas[key].AnyOf.All(s => s.Enum.Any()));
    }
}