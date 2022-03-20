// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MingweiSamuel;
using MingweiSamuel.Lcu;
using RiotGames.Help;
using RiotGames.Help2Swagger;

Console.WriteLine("Hello, World!");

string[] postTypes =
{
    "string", "uint32", "uint64", "int32", "int64", "double", "float", "vector of object", "vector of uint32",
    "map of object"
};


using var client = new HttpClient();
var helpConsole = await client.GetFromJsonAsync<HelpConsoleSchema>("https://www.mingweisamuel.com/lcu-schema/lcu/help.console.json");
var helpFull = await client.GetFromJsonAsync<HelpFullSchema>("https://www.mingweisamuel.com/lcu-schema/lcu/help.json");

var openApi = new LcuApiOpenApiSchema
{
    Paths = new Dictionary<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
        OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
        OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>(),
    Components = new OpenApiComponentsObject<LcuComponentSchemaObject, LcuComponentPropertyObject>
    {
        Schemas = new Dictionary<string, LcuComponentSchemaObject>()
    }
};

var typeNames = helpConsole!.Types.Keys.ToArray();


var httpFunctions = helpConsole.Functions.Where(f => f.Value.HttpMethod != null).ToArray();

var otherFunctions = helpConsole.Functions.Where(f => !httpFunctions.Contains(f)).ToArray();

var httpFunctionsByUrl = httpFunctions.GroupBy(f => f.Value.Url!);

foreach (var urlFunctions in httpFunctionsByUrl)
{
    var url = urlFunctions.Key;

    var pathObject =
        new OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
            OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
            OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>();

    foreach (var function in urlFunctions)
        switch (function.Value.HttpMethod)
        {
            case "GET":
                pathObject.Get = FunctionToMethodObject(function);
                break;
            case "POST":
                pathObject.Post = FunctionToMethodObject(function);
                break;
            case "PUT":
                pathObject.Put = FunctionToMethodObject(function);
                break;
            case "DELETE":
                pathObject.Delete = FunctionToMethodObject(function);
                break;
        }

    openApi.Paths.Add(url, pathObject);
}

foreach (var function in otherFunctions)
{
    var pathObject =
        new OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
            OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
            OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>
        {
            Post = FunctionToMethodObject(function)
        };

    openApi.Paths.Add(function.Key, pathObject);
}

foreach (var (typeIdentifier, typeSchema) in helpConsole.Types)
{
    var schema = new LcuComponentSchemaObject
    {
        Description = typeSchema.Description
    };

    if (typeSchema.Fields != null)
    {
        schema.Type = "object";
        schema.Properties = new Dictionary<string, LcuComponentPropertyObject>();

        foreach (var (fieldIdentifier, fieldSchema) in typeSchema.Fields.SelectMany(d => d).DistinctBy(f => f.Key))
        {
            var property = new LcuComponentPropertyObject();
            if (!string.IsNullOrEmpty(fieldSchema.Description))
                property.Description = fieldSchema.Description;
            switch (fieldSchema.Type)
            {
                case string stringType:
                    switch (stringType)
                    {
                        case "string":
                            property.Type = stringType;
                            break;
                        case "bool":
                            property.Type = "boolean";
                            break;
                        case "object":
                            property.AdditionalProperties = true;
                            property.Type = stringType;
                            break;
                        case "double":
                            property.Type = "number";
                            property.Format = "double";
                            break;
                        case "float":
                            property.Type = "number";
                            property.Format = "float";
                            break;
                        default:
                        {
                            if (stringType.StartsWith("uint") || stringType.StartsWith("int"))
                            {
                                property.Type = "integer";
                                property.Format = stringType.TrimStart('u');
                            }
                            else
                            {
                                if (stringType.StartsWith("vector of "))
                                {
                                    property.Items = new LcuComponentPropertyObject();
                                    property.Type = "array";
                                    var ofType = stringType.Remove(0, "vector of ".Length);

                                    if (typeNames.Contains(ofType))
                                    {
                                        property.Items.Ref = "#/components/schemas/" + ofType;
                                    }
                                    else
                                    {
                                        if (ofType is "object" or "string")
                                        {
                                            property.Items.Type = ofType;
                                        }
                                        else if (ofType.StartsWith("uint") || ofType.StartsWith("int"))
                                        {
                                            property.Items.Type = "integer";
                                            property.Items.Format = ofType.TrimStart('u');
                                        }
                                        else
                                        {
                                            Debugger.Break();
                                        }
                                    }
                                }
                                else if (stringType.StartsWith("map of "))
                                {
                                    property.Items = new LcuComponentPropertyObject();
                                    property.Type = "array";
                                    var ofType = stringType.Remove(0, "map of ".Length);
                                    if (typeNames.Contains(ofType))
                                    {
                                        property.Items.Ref = "#/components/schemas/" + ofType;
                                    }
                                    else
                                    {
                                        switch (ofType)
                                        {
                                            case "object" or "string":
                                                property.Items.Type = ofType;
                                                break;
                                            case "bool":
                                                property.Items.Type = "boolean";
                                                break;
                                            case "double":
                                                property.Items.Type = "number";
                                                property.Items.Format = "double";
                                                break;
                                            default:
                                            {
                                                if (ofType.StartsWith("uint") || ofType.StartsWith("int"))
                                                {
                                                    property.Items.Type = "integer";
                                                    property.Items.Format = ofType.TrimStart('u');
                                                }
                                                else
                                                {
                                                    Debugger.Break();
                                                    throw new Exception("Unknown type");
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Debugger.Break();
                                }
                            }

                            break;
                        }
                    }

                    break;
                case Dictionary<string, HelpConsoleTypeSchema> typeType:
                    property.Ref = "#/components/schemas/" + typeType.Single().Key;
                    break;
                default:
                    Debugger.Break();
                    break;
            }

            schema.Properties.Add(fieldIdentifier, property);
        }
    }
    else if (typeSchema.Values != null)
    {
        schema.Type = "string";
        schema.Enum = typeSchema.Values.Select(v => v.Name).ToArray();
    }
    else
    {
        Debugger.Break();
        throw new NotImplementedException("Unknown component type");
    }

    openApi.Components.Schemas.Add('/' + typeIdentifier, schema);
}

var openApiJson = JsonSerializer.Serialize(openApi,
    new JsonSerializerOptions {WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull});

Console.ReadKey();


OpenApiMethodObject<LcuParameterObject, LcuSchemaObject> FunctionToMethodObject(
    KeyValuePair<string, HelpConsoleFunctionSchema> function)
{
    var (functionIdentifier, functionSchema) = function;
    var method = new OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>
    {
        OperationId = functionIdentifier,
        Description = functionSchema.Help,
        Summary = functionSchema.Description
    };

    if (functionSchema.Arguments.Any())
    {
        var parameters = new List<LcuParameterObject>();
        foreach (var (argumentIdentifier, argumentSchema) in functionSchema.Arguments.SelectMany(a => a))
        {
            var parameter = new LcuParameterObject
            {
                Name = argumentIdentifier,
                Required = argumentSchema.Optional == false
            };

            if (functionSchema.HttpMethod == "GET")
            {
                parameter.In =
                    functionSchema.Url!.Contains($"{{{argumentIdentifier}}}") ? "path" : "query"; // And body? HelpConsoleTypeSchema
            }
            else if (functionSchema.Url != null && (functionSchema.Url.Contains($"{{{argumentIdentifier}}}") || functionSchema.Url.Contains($"{{+{argumentIdentifier}}}")))
            {
                parameter.In = "path";
            }
            else if (functionSchema.Usage.Contains($"[{argumentIdentifier}]") || functionSchema.Usage.Contains($"[<{argumentIdentifier}>]") ||
                     functionSchema.Arguments.Length > 1 &&
                     functionSchema.Arguments.All(a => postTypes.Contains(a.Single().Value.Type as string)))
            {
                parameter.In = "query";
            }
            else
            {
                if (method.RequestBody != null)
                    throw new Exception("RequestBody already set!");

                var contentSchema = TypeToLcuSchemaObject(argumentSchema.Type);

                method.RequestBody = new OpenApiResponseObject<LcuSchemaObject>
                {
                    Content = new Dictionary<string, OpenApiContentObject<LcuSchemaObject>>
                    {
                        {
                            "application/json", new OpenApiContentObject<LcuSchemaObject>
                            {
                                Schema = contentSchema
                            }
                        }
                    }
                };

                continue; // And add request body.
            }

            switch (argumentSchema.Type)
            {
                case string stringValue:
                    if (stringValue.StartsWith("uint") || stringValue.StartsWith("int"))
                    {
                        parameter.Type = "integer";
                        parameter.Format = stringValue.TrimStart('u');
                    }
                    else
                        parameter.Type = stringValue;
                    break;
                case Dictionary<string, HelpConsoleTypeSchema> typeValue:
                    if (typeValue.Single().Value.Values != null) // Enum
                    {
                        parameter.Type = "string";
                        parameter.Enum = typeValue.Single().Value.Values!.Select(v => v.Name).ToArray();
                    }

                    break;
            }

            parameters.Add(parameter);
        }

        method.Parameters = parameters.ToArray();
    }

    method.Responses = new Dictionary<string, OpenApiResponseObject<LcuSchemaObject>>();
    if (functionSchema.Returns != null)
    {
        var contentSchema = TypeToLcuSchemaObject(functionSchema.Returns);

        method.Responses.Add("200", new OpenApiResponseObject<LcuSchemaObject>
        {
            Description = "Successful response",
            Content = new Dictionary<string, OpenApiContentObject<LcuSchemaObject>>
            {
                {
                    "application/json", new OpenApiContentObject<LcuSchemaObject>
                    {
                        Schema = contentSchema
                    }
                }
            }
        });
    }
    else
    {
        method.Responses.Add("204", new OpenApiResponseObject<LcuSchemaObject>
        {
            Description = "No content"
        });
    }

    method.Tags = helpFull!.Functions.Single(f => f.Name == functionIdentifier).Tags
        .Where(t => t != "$remoting-binding-module").ToArray();

    return method;
}

LcuSchemaObject TypeToLcuSchemaObject(object type)
{
    LcuSchemaObject contentSchema;
    switch (type)
    {
        case string stringValue:
            if (stringValue.StartsWith("vector of "))
            {
                contentSchema = new LcuSchemaObject
                {
                    Type = "array",
                    Items = new OpenApiSchemaObject
                    {
                        Ref = "#/components/schemas/" + stringValue.Remove(0, "vector of ".Length)
                    }
                };
            }
            else if (stringValue.StartsWith("map of "))
            {
                var typeName = stringValue.Remove(0, "map of ".Length);
                contentSchema = new LcuSchemaObject
                {
                    Type = "array"
                };
                if (typeName == "object")
                    contentSchema.Items = new OpenApiSchemaObject
                    {
                        Type = "object"
                    };
            }
            else switch (stringValue)
            {
                case "object":
                    contentSchema = new LcuSchemaObject
                    {
                        Type = "object",
                        AdditionalProperties = true
                    };
                    break;
                case "bool":
                    contentSchema = new LcuSchemaObject
                    {
                        Type = "boolean"
                    };
                    break;
                case "string":
                    contentSchema = new LcuSchemaObject
                    {
                        Type = stringValue
                    };
                    break;
                default:
                {
                    if (stringValue.StartsWith("int") || stringValue.StartsWith("uint"))
                    {
                        contentSchema = new LcuSchemaObject
                        {
                            Type = "integer",
                            Format = stringValue
                        };
                    }
                    else if (stringValue == "double")
                    {
                        contentSchema = new LcuSchemaObject
                        {
                            Type = "number",
                            Format = stringValue
                        };
                    }
                    else
                    {
                        Debugger.Break();
                        throw new Exception();
                    }

                    break;
                }
            }

            break;
        case Dictionary<string, HelpConsoleTypeSchema> dictionaryValue:
            contentSchema = new LcuSchemaObject
            {
                Ref = "#/components/schemas/" + dictionaryValue.Single().Key
            };
            break;
        default:
            throw new NotImplementedException("Not sure what happened.");
    }

    return contentSchema;
}