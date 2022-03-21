using System.Diagnostics;
using System.Text.Json.Serialization;
using MingweiSamuel.Lcu;

namespace MingweiSamuel;

internal class OpenApiSchema<
    TPath, TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema,
    TComponents, TComponentSchema, TComponentProperty>
    where TPath : OpenApiPathObject<TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema>
    where TGetMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPostMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPutMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TParameter : OpenApiParameterObject
    where TSchema : OpenApiSchemaObject
    where TComponents : OpenApiComponentsObject<TComponentSchema, TComponentProperty>
    where TComponentSchema : OpenApiComponentSchemaObject<TComponentProperty>
    where TComponentProperty : OpenApiComponentPropertyObject
{
    [JsonPropertyOrder(1)]
    public TComponents? Components { get; set; }
    [JsonPropertyOrder(2)]
    public Dictionary<string, TPath>? Paths { get; set; }
}

#region Open API Paths

internal class OpenApiPathObject<TGetMethodObject, TPostMethodObject, TPutMethodObject, TParameter, TSchema>
    where TGetMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPostMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TPutMethodObject : OpenApiMethodObject<TParameter, TSchema>
    where TParameter : OpenApiParameterObject
    where TSchema : OpenApiSchemaObject
{
    public OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>? Delete { get; set; }
    public TGetMethodObject? Get { get; set; }
    public OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>? Patch { get; set; }
    public TPostMethodObject? Post { get; set; }
    public TPutMethodObject? Put { get; set; }
}

internal class OpenApiMethodObject<TParameter, TSchema>
    where TParameter : OpenApiParameterObject
    where TSchema : OpenApiSchemaObject
{
    public string? Description { get; set; }
    public string? OperationId { get; set; }
    public TParameter[]? Parameters { get; set; }
    public OpenApiResponseObject<TSchema>? RequestBody { get; set; }
    public Dictionary<string, OpenApiResponseObject<TSchema>>? Responses { get; set; }
    public string? Summary { get; set; }
    public string[]? Tags { get; set; }
}

internal class OpenApiParameterObject
{
    public bool? AdditionalProperties { get; set; }
    public string? Description { get; set; }
    public string? Format { get; set; }
    public string[]? Enum { get; set; }
    public string? In { get; set; }
    public OpenApiParameterObject? Items { get; set; }
    public string? Name { get; set; }
    [JsonPropertyName("$ref")] public string? Ref { get; set; }
    public bool? Required { get; set; }
    public string? Type { get; set; }
}

[DebuggerDisplay("Description = {Description}")]
internal class OpenApiResponseObject<TSchema>
    where TSchema : OpenApiSchemaObject
{
    public Dictionary<string, OpenApiContentObject<TSchema>>? Content { get; set; }
    public string? Description { get; set; }
}

internal class OpenApiContentObject<TSchema>
    where TSchema : OpenApiSchemaObject
{
    public TSchema? Schema { get; set; }
}

internal class OpenApiSchemaObject
{
    public object? AdditionalProperties { get; set; }
    public string? Description { get; set; }
    public string? Format { get; set; }
    public OpenApiSchemaObject? Items { get; set; }
    [JsonPropertyName("$ref")] public string? Ref { get; set; }
    public string? Type { get; set; }
}

#endregion Open API Paths

#region Open API Components

[DebuggerDisplay("Schemas = {Schemas.Count}")]
internal class OpenApiComponentsObject<TComponentSchema, TComponentProperty>
    where TComponentSchema : OpenApiComponentSchemaObject<TComponentProperty>
    where TComponentProperty : OpenApiComponentPropertyObject
{
    public Dictionary<string, TComponentSchema>? Schemas { get; set; }
}

internal class OpenApiComponentSchemaObject<TProperty>
    where TProperty : OpenApiComponentPropertyObject
{
    public Dictionary<string, TProperty>? Properties { get; set; }
    public string? Type { get; set; }
}

[DebuggerDisplay("Type = {Type} $ref = {Ref}")]
internal class OpenApiComponentPropertyObject
{
    [JsonPropertyName("$ref")] public string? Ref { get; set; }
    public bool? AdditionalProperties { get; set; }
    public string[]? Enum { get; set; }
    public string? Format { get; set; }
    public OpenApiComponentPropertyObject? Items { get; set; }
    public string? Type { get; set; }
}

#endregion Open API Component