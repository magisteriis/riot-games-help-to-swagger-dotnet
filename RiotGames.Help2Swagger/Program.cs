﻿// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RiotGames.Help;
using RiotGames.Help2Swagger;
using RiotGames.Help2Swagger.Converters;

Console.WriteLine("Help2Swagger");

string? outPath = null;
HelpHttpLocations? helpHttpLocations = null;
switch (args.Length)
{
    case 0:
        Console.WriteLine(
            "No arguments specified. Downloading LCU Help from default locations and doesn't output anything.");
        helpHttpLocations = HelpHttpLocations.Lcu;
        break;
    case 1:
        Console.WriteLine("Custom out-path set.");
        outPath = args[0];
        break;
    case 2:
        Console.WriteLine("Downloading Help from custom locations.");
        if (args[0].ToLower() is "lcu" or "rcs")
        {
            helpHttpLocations = args[0].ToLower() switch
            {
                "lcu" => HelpHttpLocations.Lcu,
                "rcs" => HelpHttpLocations.Rcs,
                _ => throw new ArgumentOutOfRangeException()
            };
            outPath = args[1];
        }
        else
        {
            helpHttpLocations = new HelpHttpLocations(args[0], args[1]);
        }

        break;
    case 3:
        Console.WriteLine("Custom Help locations, and an output path set.");
        helpHttpLocations = new HelpHttpLocations(args[0], args[1]);
        outPath = args[2];
        break;
    default:
        Console.WriteLine("This many arguments isn't supported.");
        Environment.Exit(1);
        break;
}


using var client = new HttpClient();
var helpConsole = await client.GetFromJsonAsync<HelpConsoleDocument>(helpHttpLocations!.HelpConsoleUrl);
var helpFull = await client.GetFromJsonAsync<HelpFullDocument>(helpHttpLocations!.HelpFullUrl);

OpenApiDocument openApi;
if (helpHttpLocations.HelpFullUrl.AbsoluteUri.Contains("lcu/help"))
    openApi = new LcuOpenApiDocument();
else if (helpHttpLocations.HelpFullUrl.AbsoluteUri.Contains("rcs/help"))
    openApi = new RcsOpenApiDocument();
else
    openApi = new OpenApiDocument
    {
        Info = new OpenApiInfo
        {
            Title = "",
            Version = "1.0.0"
        },
        Paths = new OpenApiPaths(),
        Components = new OpenApiComponents()
    };

//openApi.SecurityRequirements.

var typeNames = helpConsole!.Types.Keys.ToArray();

var httpFunctions = helpConsole.Functions.Where(f => f.Value.HttpMethod != null).ToArray();

var otherFunctions = helpConsole.Functions.Where(f => !httpFunctions.Contains(f)).ToArray();

var httpFunctionsByUrl = httpFunctions.GroupBy(f => f.Value.Url!);

foreach (var (typeIdentifier, typeSchema) in helpConsole.Types.OrderBy(t => t.Key))
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
    else if (typeSchema.Values != null) // TODO: oneOf string, integer
    {
        schema.Type = "string";
        schema.Enum.AddStrings(typeSchema.Values.Select(v => v.Name));
    }
    else
    {
        Debugger.Break();
        throw new NotImplementedException("Unknown schema type");
    }

    openApi.Components.Schemas.Add(typeIdentifier, schema);
}

foreach (var function in otherFunctions.OrderBy(f => f.Key))
{
    var pathObject = new OpenApiPathItem
    {
        Operations =
        {
            [OperationType.Post] = OperationConverter.Convert(function, openApi, helpFull!)
        }
    };

    openApi.Paths.Add('/' + function.Key, pathObject);
}

foreach (var urlFunctions in httpFunctionsByUrl.OrderBy(g => g.Key))
{
    var url = urlFunctions.Key;

    var pathObject = new OpenApiPathItem();

    foreach (var function in urlFunctions.OrderBy(f => f.Key))
    {
        var operation = OperationConverter.Convert(function, openApi, helpFull!);
        pathObject.AddOperation(function.Value.HttpMethod!, operation);
    }

    openApi.Paths.Add(url, pathObject);
}

foreach (var (path, operations) in openApi.Paths
             .Select(p => (p.Key, p.Value.Operations.Where(o => !o.Value.Tags.Any())))
             .Where(x => x.Item2.Any()))
foreach (var operation in operations)
    operation.Value.Tags.Add(path.Split('/', StringSplitOptions.RemoveEmptyEntries).First());

// Customize the tag sort order in Swagger UI
openApi.Tags = openApi.Paths
    .SelectMany(p => p.Value.Operations)
    .SelectMany(o => o.Value.Tags)
    .DistinctBy(t => t.Name)
    .OrderBy(t => t.Name.StartsWith("Plugin"))
    .ThenBy(t => !t.Name.StartsWith("Plugins"))
    .ThenBy(t => !t.Name.StartsWith("Plugin Manager"))
    .ThenBy(t => !t.Name.StartsWith("Plugin Asset"))
    .ThenBy(t => t.Name.StartsWith("Plugin lol-"))
    .ThenBy(t => t.Name)
    .ToList();


if (outPath != null)
{
    new FileInfo(outPath).Directory!.Create();
    switch (outPath.Split('.', StringSplitOptions.RemoveEmptyEntries).Last().ToLower())
    {
        case "json":
            await openApi.WriteAsV3JsonAsync(outPath);
            break;
        case "yaml":
        case "yml":
            await openApi.WriteAsV3YamlAsync(outPath);
            break;
        default:
            await openApi.WriteAsV3JsonAsync(Path.Combine(outPath, "openapi.json"));
            await openApi.WriteAsV3YamlAsync(Path.Combine(outPath, "openapi.yaml"));
            break;
    }
}

Console.WriteLine("Done!");
Debugger.Break();