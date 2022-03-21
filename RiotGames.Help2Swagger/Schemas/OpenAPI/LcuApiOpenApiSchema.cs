namespace MingweiSamuel.Lcu;

using LcuMethod = OpenApiMethodObject<LcuParameterObject, LcuSchemaObject>;
using LcuComponentsObject = OpenApiComponentsObject<LcuComponentSchemaObject, LcuComponentPropertyObject>;

internal class LcuApiOpenApiSchema : OpenApiSchema<
    OpenApiPathObject<LcuMethod, LcuMethod, LcuMethod, LcuParameterObject, LcuSchemaObject>,
    LcuMethod, LcuMethod, LcuMethod, LcuParameterObject, LcuSchemaObject, LcuComponentsObject,
    LcuComponentSchemaObject, LcuComponentPropertyObject>
{
}

internal class LcuSchemaObject : OpenApiSchemaObject
{
}

internal class LcuParameterObject : OpenApiParameterObject
{
}

internal class LcuComponentSchemaObject : OpenApiComponentSchemaObject<LcuComponentPropertyObject>
{
    public string? Description { get; set; }
    public string[]? Enum { get; set; }
}

internal class LcuComponentPropertyObject : OpenApiComponentPropertyObject
{
    public object? AdditionalProperties { get; set; }
    public string? Description { get; set; }
}