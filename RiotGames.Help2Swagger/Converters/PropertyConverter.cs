using System.Diagnostics;
using Microsoft.OpenApi.Models;
using RiotGames.Help;

namespace RiotGames.Help2Swagger.Converters;

internal static class PropertyConverter
{
    public static OpenApiSchema Convert(HelpConsoleTypeField fieldSchema, string[] typeNames, OpenApiDocument openApi)
    {
        var property = new OpenApiSchema();
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
                        property.AdditionalPropertiesAllowed = true;
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
                                property.Items = new OpenApiSchema();
                                property.Type = "array";
                                var ofType = stringType.Remove(0, "vector of ".Length);

                                if (typeNames.Contains(ofType))
                                {
                                    property.Items.Reference = new OpenApiReference
                                        {Type = ReferenceType.Schema, Id = ofType};
                                }
                                else
                                {
                                    if (ofType is "object" or "string")
                                    {
                                        property.Items.Type = ofType;
                                        if (ofType == "object")
                                            property.Items.AdditionalPropertiesAllowed = true;
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
                                property.AdditionalProperties = new OpenApiSchema();
                                property.Type = "object";
                                var ofType = stringType.Remove(0, "map of ".Length);
                                if (typeNames.Contains(ofType))
                                    property.AdditionalProperties.Reference = new OpenApiReference
                                        {Type = ReferenceType.Schema, Id = ofType};
                                else
                                    switch (ofType)
                                    {
                                        case "object":
                                        case "": // RCS, unspecified type.
                                        case "map of ": // RCS, double depth map?
                                            property.AdditionalProperties.Type = ofType;
                                            break;
                                        case "string":
                                            property.AdditionalProperties.Type = "string";
                                            break;
                                        case "bool":
                                            property.AdditionalProperties.Type = "boolean";
                                            break;
                                        case "double":
                                            property.AdditionalProperties.Type = "number";
                                            property.AdditionalProperties.Format = "double";
                                            break;
                                        default:
                                        {
                                            if (ofType.StartsWith("uint") || ofType.StartsWith("int"))
                                            {
                                                property.AdditionalProperties.Type = "integer";
                                                property.AdditionalProperties.Format = ofType.TrimStart('u');
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
                            else
                            {
                                Debugger.Break();
                            }
                        }

                        break;
                    }
                }

                break;
            case Dictionary<string, HelpConsoleType> typeType:
                property.Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = typeType.Single().Key};
                break;
            default:
                Debugger.Break();
                break;
        }

        return property;
    }
}