using Microsoft.OpenApi.Models;

namespace RiotGames.Help2Swagger.Extensions;

internal static class OpenApiOperationExtensions
{
    public static void AddRequestBodyJson(this OpenApiOperation operation, OpenApiMediaType mediaTypeObject)
    {
        operation.RequestBody ??= new OpenApiRequestBody();
        operation.RequestBody.Content["application/json"] = mediaTypeObject;
    }

    public static void AddRequestBodyJson(this OpenApiOperation operation, OpenApiSchema schema)
    {
        operation.AddRequestBodyJson(new OpenApiMediaType {Schema = schema});
    }

    public static void AddRequestBodyYaml(this OpenApiOperation operation, OpenApiMediaType mediaTypeObject)
    {
        operation.RequestBody ??= new OpenApiRequestBody();
        operation.RequestBody.Content["application/x-yaml"] = mediaTypeObject;
    }

    public static void AddRequestBodyYaml(this OpenApiOperation operation, OpenApiSchema schema)
    {
        operation.AddRequestBodyYaml(new OpenApiMediaType {Schema = schema});
    }

    public static void AddRequestBodyMsgPack(this OpenApiOperation operation, OpenApiMediaType mediaTypeObject)
    {
        operation.RequestBody ??= new OpenApiRequestBody();
        operation.RequestBody.Content["application/x-msgpack"] = mediaTypeObject;
    }

    public static void AddRequestBodyMsgPack(this OpenApiOperation operation, OpenApiSchema schema)
    {
        operation.AddRequestBodyMsgPack(new OpenApiMediaType {Schema = schema});
    }
}