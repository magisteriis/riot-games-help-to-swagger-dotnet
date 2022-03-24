using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger;

internal static class OpenApiResponsesExtensions
{
    private const string STATUS_CODE_OK = "200";
    private const string STATUS_CODE_NO_CONTENT = "204";
    private const string MEDIA_TYPE_JSON = "application/json";
    private const string MEDIA_TYPE_YAML = "application/x-yaml";
    private const string MEDIA_TYPE_MSGPACK = "application/x-msgpack";

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
        if (!responses.ContainsKey(STATUS_CODE_OK))
            responses.Add(STATUS_CODE_OK, new OpenApiResponse
            {
                Description = description,
                Content =
                {
                    [mediaType] = mediaTypeObject
                }
            });
        else
            responses[STATUS_CODE_OK].Content[mediaType] = mediaTypeObject;
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
    public static void AddOkYaml(this OpenApiResponses responses, string description,
        OpenApiMediaType mediaTypeObject)
    {
        responses.AddOk(description, MEDIA_TYPE_YAML, mediaTypeObject);
    }

    [DebuggerStepThrough]
    public static void AddOkYaml(this OpenApiResponses responses, string description, OpenApiSchema schema)
    {
        responses.AddOk(description, MEDIA_TYPE_YAML, schema);
    }

    [DebuggerStepThrough]
    public static void AddOkYaml(this OpenApiResponses responses, OpenApiSchema schema)
    {
        responses.AddOkYaml("Successful response", schema);
    }

    [DebuggerStepThrough]
    public static void AddOkMsgPack(this OpenApiResponses responses, string description,
        OpenApiMediaType mediaTypeObject)
    {
        responses.AddOk(description, MEDIA_TYPE_MSGPACK, mediaTypeObject);
    }

    [DebuggerStepThrough]
    public static void AddOkMsgPack(this OpenApiResponses responses, string description, OpenApiSchema schema)
    {
        responses.AddOk(description, MEDIA_TYPE_MSGPACK, schema);
    }

    [DebuggerStepThrough]
    public static void AddOkMsgPack(this OpenApiResponses responses, OpenApiSchema schema)
    {
        responses.AddOkMsgPack("Successful response", schema);
    }
}