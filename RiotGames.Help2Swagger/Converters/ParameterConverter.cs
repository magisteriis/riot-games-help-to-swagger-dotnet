using System.Diagnostics;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using RiotGames.Help;

namespace RiotGames.Help2Swagger.Converters;

internal static class ParameterConverter
{
    private static readonly string[] queryTypes =
    {
        "string", "uint32", "uint64", "int32", "int64", "double", "float", "bool",
        "vector of string", "map of string", "vector of uint64", "vector of int64",
        "" // RCS
        //, "vector of object", "vector of uint32",
        //"map of object"
    };

    private static readonly string[] headerParameters = {"jwt", "if-none-match"};

    public static OpenApiParameter? Convert(
        KeyValuePair<string, HelpConsoleFunctionArgument> argument,
        HelpConsoleFunction functionSchema,
        OpenApiOperation operation,
        OpenApiDocument openApi)
    {
        var (argumentIdentifier, argumentSchema) = argument;

        var parameter = new OpenApiParameter
        {
            Name = argumentIdentifier,
            Required = argumentSchema.Optional == false
        };

        if (!string.IsNullOrEmpty(argumentSchema.Description))
            parameter.Description = argumentSchema.Description;

        if (headerParameters.Contains(argumentIdentifier))
        {
            parameter.In = ParameterLocation.Header;
        }
        else if (functionSchema.Url != null && (functionSchema.Url!.Contains($"{{{argumentIdentifier}}}") ||
                                                functionSchema.Url.Contains($"{{+{argumentIdentifier}}}")))
        {
            parameter.In = ParameterLocation.Path;
        }
        else if (functionSchema.Arguments.Length ==
                 functionSchema.Arguments.Count(a => queryTypes.Contains(a.Single().Value.Type as string)) &&
                 queryTypes.Contains(argumentSchema.Type as string))
        {
            parameter.In = ParameterLocation.Query;
        }
        //else if (functionSchema.HttpMethod == "GET") // RCS has bodies for GET.....................
        //{
        //    parameter.In = ParameterLocation.Query;
        //}
        else if ((functionSchema.Usage.Contains($"[{argumentIdentifier}]") ||
                  functionSchema.Usage.Contains($"[<{argumentIdentifier}>]") ||
                  functionSchema.Arguments.Length > 1) &&
                 queryTypes.Contains(argumentSchema.Type as string) ||
                 functionSchema.Arguments.All(a => queryTypes.Contains(a.Single().Value.Type as string)))
        {
            parameter.In = ParameterLocation.Query;
        }
        else if (argumentSchema.Type is Dictionary<string, HelpConsoleType> argumentSchemaTypeSchema &&
                 openApi.Components.Schemas.ContainsEnum(argumentSchemaTypeSchema.Single().Key))
        {
            parameter.In = ParameterLocation.Query;
        }
        else
        {
            if (operation.RequestBody != null)
                throw new Exception("RequestBody already set!");

            var contentSchema = SchemaConverter.Convert(argumentSchema.Type, openApi);

            operation.AddRequestBodyJson(contentSchema);
            operation.AddRequestBodyYaml(contentSchema);
            operation.AddRequestBodyMsgPack(contentSchema);

            return null; // And add request body.
        }


        var schema = new OpenApiSchema();
        parameter.Schema = schema;

        switch (argumentSchema.Type)
        {
            case string stringValue:
                if (stringValue.StartsWith("vector of "))
                {
                    schema.Type = "array";
                    schema.Items = new OpenApiSchema();
                    var ofType = stringValue.Remove(0, "vector of ".Length);
                    if (openApi.Components.Schemas.Keys.Contains(ofType))
                    {
                        schema.Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = ofType};
                    }
                    else if (ofType.StartsWith("uint") || ofType.StartsWith("int"))
                    {
                        schema.Items.Type = "integer";
                        schema.Items.Format = ofType;
                    }
                    else if (ofType == "string")
                    {
                        schema.Items.Type = "string";
                    }
                    else if (ofType == "object")
                    {
                        schema.Items.Type = "object";
                        schema.Items.AdditionalPropertiesAllowed = true;
                    }
                    else
                    {
                        Debugger.Break();
                        throw new Exception("Unexpected parameter item type");
                    }
                }
                else if (stringValue.StartsWith("uint") || stringValue.StartsWith("int"))
                {
                    schema.Type = "integer";
                    schema.Format = stringValue;
                }
                else
                {
                    switch (stringValue)
                    {
                        case "double" or "float":
                            schema.Type = "number";
                            schema.Format = stringValue;
                            break;
                        case "bool":
                            schema.Type = "boolean";
                            break;
                        default:
                            schema.Type = stringValue;
                            break;
                    }
                }

                break;
            case Dictionary<string, HelpConsoleType> typeValue:
                if (typeValue.Single().Value.Values != null ||
                    openApi.Components.Schemas[typeValue.Keys.Single()].Enum.Any()) // Enum
                {
                    schema.Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = typeValue.Single().Key};
                }
                else
                {
                    Debugger.Break();
                    throw new Exception("Unknown parameter type");
                }

                break;
        }

        return parameter;
    }
}
