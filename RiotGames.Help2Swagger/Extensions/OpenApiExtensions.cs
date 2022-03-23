using System.Diagnostics;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger.Extensions;

internal static class OpenApiExtensions
{
    private const string STATUS_CODE_OK = "200";
    private const string STATUS_CODE_NO_CONTENT = "204";
    private const string MEDIA_TYPE_JSON = "application/json";

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
    public static void AddNoContent(this OpenApiResponses responses)
    {
        responses.Add(STATUS_CODE_NO_CONTENT, new OpenApiResponse
        {
            Description = "No content"
        });
    }


    /// <param name="description">E.g. "Successful response".</param>
    /// <param name="mediaType">E.g. "application/json".</param>
    [DebuggerStepThrough]
    public static void AddOk(this OpenApiResponses responses, string description, string mediaType,
        OpenApiMediaType mediaTypeObject)
    {
        responses.Add(STATUS_CODE_OK, new OpenApiResponse
        {
            Description = description,
            Content =
            {
                [mediaType] = mediaTypeObject
            }
        });
    }

    /// <param name="description">E.g. "Successful response".</param>
    /// <param name="mediaType">E.g. "application/json".</param>
    [DebuggerStepThrough]
    public static void AddOk(this OpenApiResponses responses, string description, string mediaType,
        OpenApiSchema schema)
    {
        responses.AddOk(description, mediaType, new OpenApiMediaType {Schema = schema});
    }

    [DebuggerStepThrough]
    public static void AddOkJson(this OpenApiResponses responses, string description,
        OpenApiMediaType mediaTypeObject)
    {
        responses.AddOk(description, MEDIA_TYPE_JSON, mediaTypeObject);
    }

    [DebuggerStepThrough]
    public static void AddOkJson(this OpenApiResponses responses, string description, OpenApiSchema schema)
    {
        responses.AddOk(description, MEDIA_TYPE_JSON, schema);
    }

    [DebuggerStepThrough]
    public static void AddOkJson(this OpenApiResponses responses, OpenApiSchema schema)
    {
        responses.AddOkJson("Successful response", schema);
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