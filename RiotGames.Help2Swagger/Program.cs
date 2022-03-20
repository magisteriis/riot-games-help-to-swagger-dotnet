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

using HttpClient client = new HttpClient();
var help = await client.GetFromJsonAsync<HelpConsoleSchema>("https://gist.githubusercontent.com/mikaeldui/57d57b4bc4d1a9e606fa6bb00e7ebf4a/raw/1817255f2d273fa46ab61a500c8e8c6823bac422/help.console.20220322.json");

var openApi = new LcuApiOpenApiSchema();

openApi.Paths = new Dictionary<string, OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>>();

var httpFunctions = help.Functions.Where(f => f.Value.HttpMethod != null).ToArray();

var httpFunctionsByUrl = httpFunctions.GroupBy(f => f.Value.Url);

foreach (var urlFunctions in httpFunctionsByUrl)
{
    var url = urlFunctions.Key;

    var pathObject =
        new OpenApiPathObject<OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
            OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>,
            OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>, LcuParameterObject, LcuSchemaObject>();

    foreach (var function in urlFunctions)
    {
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
    }

    openApi.Paths.Add(url, pathObject);
}

var openApiJson = JsonSerializer.Serialize(openApi, new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

Console.ReadKey();


OpenApiMethodObject<LcuParameterObject, LcuSchemaObject> FunctionToMethodObject(
    KeyValuePair<string, HelpConsoleFunctionSchema> function)
{
    var method = new OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>()
    {
        OperationId = function.Key,
        Description = function.Value.Description
    };

    var schema = function.Value;

    if (schema.Arguments.Any())
    {
        var parameters = new List<LcuParameterObject>();
        foreach (var argument in schema.Arguments.SelectMany(a => a))
        {
            var parameter = new LcuParameterObject();
            parameter.Name = argument.Key;
            parameter.Required = argument.Value.Optional == false;

            if (schema.HttpMethod == "GET")
                parameter.In = schema.Url.Contains($"{{{argument.Key}}}") ? "path" : "query"; // And body?
            else if (schema.Url.Contains($"{{{argument.Key}}}"))
                parameter.In = "path";
            else
                continue; // And add request body.

            switch (argument.Value.Type)
            {
                case string stringValue:
                    parameter.Type = stringValue;
                    break;
                case Dictionary<string, HelpConsoleTypeSchema> typeValue:
                    if (typeValue.Single().Value.Values != null) // Enum
                    {
                        parameter.Type = "string";
                        parameter.Enum = typeValue.Single().Value.Values.Select(v => v.Name).ToArray();
                    }

                    break;
            }
            parameters.Add(parameter);
        }

        method.Parameters = parameters.ToArray();
    }

    if (schema.Returns != null)
    {
        LcuSchemaObject contentSchema;
        switch (schema.Returns)
        {
            case string stringValue:
                contentSchema = new LcuSchemaObject()
                {
                    Type = "object"
                };
                if (stringValue == "object")
                    contentSchema.AdditionalProperties = true;
                else
                    Debugger.Break();

                break;
            case Dictionary<string, HelpConsoleTypeSchema> dictionaryValue:
                contentSchema = new LcuSchemaObject()
                {
                    Ref = "#/components/schemas/" + dictionaryValue.Single().Key
                };
                break;
            default:
                throw new NotImplementedException("Not sure what happened.");
        }


        method.Responses = new();
        method.Responses.Add("200", new OpenApiResponseObject<LcuSchemaObject>()
        {
            Description = "Successful response",
            Content = new Dictionary<string, OpenApiContentObject<LcuSchemaObject>>()
            {
                {
                    "application/json", new OpenApiContentObject<LcuSchemaObject>()
                    {
                        Schema = contentSchema
                    }
                }
            }
        });
    }

    return method;
}