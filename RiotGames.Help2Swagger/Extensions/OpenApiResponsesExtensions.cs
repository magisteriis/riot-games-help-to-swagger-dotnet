using System.Diagnostics;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger;

internal static class OpenApiResponsesExtensions
{
    private const string DESCRIPTION_NO_CONTENT = "No content";

    private const string DESCRIPTION_OK = "Successful response";

    [DebuggerStepThrough]
    public static void AddNoContent(this OpenApiResponses responses)
    {
        responses.AddNoContent(DESCRIPTION_NO_CONTENT);
    }

    [DebuggerStepThrough]
    public static void AddOkJson(this OpenApiResponses responses, OpenApiSchema schema)
    {
        responses.AddOkJson(DESCRIPTION_OK, schema);
    }

    [DebuggerStepThrough]
    public static void AddOkYaml(this OpenApiResponses responses, OpenApiSchema schema)
    {
        responses.AddOkYaml(DESCRIPTION_OK, schema);
    }

    [DebuggerStepThrough]
    public static void AddOkMsgPack(this OpenApiResponses responses, OpenApiSchema schema)
    {
        responses.AddOkMsgPack(DESCRIPTION_OK, schema);
    }
}