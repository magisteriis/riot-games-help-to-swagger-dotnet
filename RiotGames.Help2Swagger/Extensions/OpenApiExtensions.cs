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


    /// <param name="description">E.g. "Successful response".</param>
    /// <param name="mediaType">E.g. "application/json".</param>
    [DebuggerStepThrough]
    public static void AddSuccess(this OpenApiResponses responses, string description, string mediaType,
        OpenApiMediaType mediaTypeObject)
    {
        responses.Add("200", new OpenApiResponse
        {
            Description = description,
            Content =
            {
                [mediaType] = mediaTypeObject
            }
        });
    }

    [DebuggerStepThrough]
    public static void AddSuccessJson(this OpenApiResponses responses, string description,
        OpenApiMediaType mediaTypeObject)
    {
        responses.AddSuccess(description, "application/json", mediaTypeObject);
    }

    [DebuggerStepThrough]
    public static void AddSuccessJson(this OpenApiResponses responses, string description, OpenApiSchema schema)
    {
        responses.AddSuccessJson(description, new OpenApiMediaType() {Schema = schema});
    }

    [DebuggerStepThrough]
    public static void AddSuccessJson(this OpenApiResponses responses, OpenApiSchema schema)
    {
        responses.AddSuccessJson("Successful response", schema);
    }
}