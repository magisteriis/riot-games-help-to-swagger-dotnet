// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using RiotGames.Help;
using RiotGames.Help2Swagger;
using RiotGames.Help2Swagger.Converters;

Console.WriteLine("Help2Swagger");

string? outPath = null;
var helpFullUrl = "https://www.mingweisamuel.com/lcu-schema/lcu/help.json";
var helpConsoleUrl = "https://www.mingweisamuel.com/lcu-schema/lcu/help.console.json";

switch (args.Length)
{
    case 0:
        Console.WriteLine(
            "No arguments specified. Downloading Help from default locations and doesn't output anything.");
        break;
    case 1:
        Console.WriteLine("Custom out-path set.");
        outPath = args[0];
        break;
    case 2:
        Console.WriteLine("Downloading Help from custom locations.");
        helpFullUrl = args[0];
        helpConsoleUrl = args[1];
        break;
    case 3:
        Console.WriteLine("Custom Help locations, and an output path set.");
        helpFullUrl = args[0];
        helpConsoleUrl = args[1];
        outPath = args[3];
        break;
    default:
        Console.WriteLine("This many arguments isn't supported.");
        Environment.Exit(1);
        break;
}


using var client = new HttpClient();
var helpConsole = await client.GetFromJsonAsync<HelpConsoleSchema>(helpConsoleUrl);
var helpFull = await client.GetFromJsonAsync<HelpFullSchema>(helpFullUrl);

var openApi = new OpenApiDocument
{
    Info = new OpenApiInfo
    {
        Title = "League Client Update",
        Version = "1.0.0-magisteriis",
        Contact = new OpenApiContact
        {
            Name = "Mikael Dúi Bolinder (DevOps Activist)",
            Url = new Uri("https://discord.gg/riotgamesdevrel")
        },
        Description = "Auto-generated from the LCU help files.",
        License = new OpenApiLicense
        {
            Name = "The Unlicense"
        }
    },
    Paths = new OpenApiPaths(),
    Components = new OpenApiComponents
    {
        SecuritySchemes =
        {
            {
                "basicAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    Description = "Username: riot. Password randomly generated on LCU start."
                }
            }
        }
    },
    SecurityRequirements =
    {
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                    {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "basicAuth"}},
                new List<string>()
            }
        }
    },
    Servers =
    {
        new OpenApiServer
        {
            Description = "YOUR local instance of LCU.",
            Url = "https://127.0.0.1:{port}",
            Variables =
            {
                {
                    "username", new OpenApiServerVariable()
                    {
                        Description = "The username, which is always \"riot\"",
                        Enum =
                        {
                            "riot"
                        }
                    }
                },
                {
                    "port", new OpenApiServerVariable()
                    {
                        Description = "The port this LCU instance is running on. Changes every restart."
                    }
                }
            }
        }
    }
};

//openApi.SecurityRequirements.

var typeNames = helpConsole!.Types.Keys.ToArray();

var httpFunctions = helpConsole.Functions.Where(f => f.Value.HttpMethod != null).ToArray();

var otherFunctions = helpConsole.Functions.Where(f => !httpFunctions.Contains(f)).ToArray();

var httpFunctionsByUrl = httpFunctions.GroupBy(f => f.Value.Url!);

foreach (var (typeIdentifier, typeSchema) in helpConsole.Types)
{
    var schema = new OpenApiSchema
    {
        Description = typeSchema.Description
    };

    if (typeSchema.Fields != null)
    {
        schema.Type = "object";

        var fields = typeSchema.Fields
            .SelectMany(d => d)
            .DistinctBy(f => f.Key)
            .OrderBy(f => f.Key);

        foreach (var (fieldIdentifier, fieldSchema) in fields)
        {
            var property = PropertyConverter.Convert(fieldSchema, typeNames, openApi);
            schema.Properties.Add(fieldIdentifier, property);
        }
    }
    else if (typeSchema.Values != null)
    {
        schema.Type = "string";
        schema.Enum = typeSchema.Values.Select(v => (IOpenApiAny) new OpenApiString(v.Name)).ToList();
    }
    else
    {
        Debugger.Break();
        throw new NotImplementedException("Unknown schema type");
    }

    openApi.Components.Schemas.Add(typeIdentifier, schema);
}

foreach (var urlFunctions in httpFunctionsByUrl)
{
    var url = urlFunctions.Key;

    var pathObject = new OpenApiPathItem();

    foreach (var function in urlFunctions)
    {
        var operation = FunctionToOperation(function);
        pathObject.AddOperation(Enum.Parse<OperationType>(function.Value.HttpMethod!, true), operation);
    }

    openApi.Paths.Add(url, pathObject);
}

foreach (var function in otherFunctions)
{
    var pathObject = new OpenApiPathItem
    {
        Operations =
        {
            [OperationType.Post] = FunctionToOperation(function)
        }
    };

    openApi.Paths.Add('/' + function.Key, pathObject);
}

var openApiJson = openApi.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

if (outPath != null)
{
    new FileInfo(outPath).Directory!.Create();
    await File.WriteAllTextAsync(outPath, openApiJson);
}

Console.WriteLine("Done!");

OpenApiOperation FunctionToOperation(
    KeyValuePair<string, HelpConsoleFunctionSchema> function)
{
    var (functionIdentifier, functionSchema) = function;
    var operation = new OpenApiOperation
    {
        OperationId = functionIdentifier,
        Description = functionSchema.Help,
        Summary = functionSchema.Description
    };

    foreach (var argument in functionSchema.Arguments.SelectMany(a => a)
             //.OrderBy(a => a.Key)
            )
    {
        var parameter = ParameterConverter.Convert(argument, functionSchema, operation, openApi);

        if (parameter == null) continue;

        if (parameter.In == ParameterLocation.Path)
            parameter.Required = true; // Microsoft.OpenApi should set this, but doesn't right now.
        else
            parameter.Required = !helpFull!.Functions
                .Single(f => f.Name == function.Key)
                .Arguments
                .Single(a => a.Name == argument.Key)
                .Optional;

        operation.Parameters.Add(parameter);
    }

    if (functionSchema.Returns != null)
    {
        var contentSchema = SchemaConverter.Convert(functionSchema.Returns, openApi);

        operation.Responses.Add("200", new OpenApiResponse
        {
            Description = "Successful response",
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = contentSchema
                }
            }
        });
    }
    else
    {
        operation.Responses.Add("204", new OpenApiResponse
        {
            Description = "No content"
        });
    }

    operation.Tags = helpFull!.Functions.Single(f => f.Name == functionIdentifier).Tags
        .Where(t => t != "$remoting-binding-module").Select(t => new OpenApiTag {Name = t}).ToList();

    return operation;
}