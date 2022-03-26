using System.Diagnostics;
using Microsoft.OpenApi.Models;
using RiotGames.Help;

namespace RiotGames.Help2Swagger.Converters;

internal static class SchemaConverter
{
    public static OpenApiSchema Convert(object type, OpenApiDocument openApi)
    {
        OpenApiSchema contentSchema;
        switch (type)
        {
            case string stringValue:
                if (stringValue.StartsWith("vector of "))
                {
                    var ofType = stringValue.Remove(0, "vector of ".Length);
                    contentSchema = new OpenApiSchema
                    {
                        Type = "array"
                    };

                    switch (ofType)
                    {
                        case "object":
                            contentSchema.Items = new OpenApiSchema
                            {
                                Type = "object",
                                AdditionalPropertiesAllowed = true
                            };
                            break;
                        case "bool":
                            contentSchema.Items = new OpenApiSchema
                            {
                                Type = "boolean"
                            };
                            break;
                        case "string":
                            contentSchema.Items = new OpenApiSchema
                            {
                                Type = ofType
                            };
                            break;
                        case "double":
                            contentSchema.Items = new OpenApiSchema
                            {
                                Type = "number",
                                Format = ofType
                            };
                            break;
                        default:
                        {
                            if (openApi.Components.Schemas.ContainsKey(ofType))
                            {
                                contentSchema.Items = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = ofType}
                                };
                            }
                            else if (ofType.StartsWith("int") || ofType.StartsWith("uint"))
                            {
                                contentSchema.Items = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Format = ofType
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
                }
                else if (stringValue.StartsWith("map of ")) // map isn't correctly handled
                {
                    var ofType = stringValue.Remove(0, "map of ".Length);
                    contentSchema = new OpenApiSchema
                    {
                        Type = "object"
                    };
                    switch (ofType)
                    {
                        case "object":
                        case "": // RCS
                        case "map of ": // RCS
                            contentSchema.AdditionalProperties = new OpenApiSchema
                            {
                                Type = "object",
                            };
                            break;
                        case "string":
                            contentSchema.AdditionalProperties = new OpenApiSchema
                            {
                                Type = "string"
                            };
                            break;
                        default:
                        {
                            if (openApi.Components.Schemas.ContainsKey(ofType))
                            {
                                contentSchema.AdditionalProperties = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = ofType}
                                };
                            }
                            else if (ofType.StartsWith("uint") || ofType.StartsWith("int"))
                            {
                                contentSchema.AdditionalProperties = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Format = ofType
                                };
                            }
                            else
                            {
                                Debugger.Break();
                                throw new Exception("Unknown map type");
                            }

                            break;
                        }
                    }
                }
                else
                {
                    switch (stringValue)
                    {
                        case "object":
                            contentSchema = new OpenApiSchema
                            {
                                Type = "object",
                                AdditionalPropertiesAllowed = true
                            };
                            break;
                        case "bool":
                            contentSchema = new OpenApiSchema
                            {
                                Type = "boolean"
                            };
                            break;
                        case "string":
                            contentSchema = new OpenApiSchema
                            {
                                Type = stringValue
                            };
                            break;
                        default:
                        {
                            if (stringValue.StartsWith("int") || stringValue.StartsWith("uint"))
                            {
                                contentSchema = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Format = stringValue
                                };
                            }
                            else if (stringValue == "double")
                            {
                                contentSchema = new OpenApiSchema
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
                }

                break;
            case Dictionary<string, HelpConsoleType> dictionaryValue:
                if (!openApi.Components.Schemas.ContainsKey(dictionaryValue.Single().Key))
                {
                    Debugger.Break();
                    throw new Exception("Unexpected type name");
                }

                contentSchema = new OpenApiSchema
                {
                    Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = dictionaryValue.Single().Key}
                };
                break;
            default:
                throw new NotImplementedException("Not sure what happened.");
        }

        return contentSchema;
    }
}
