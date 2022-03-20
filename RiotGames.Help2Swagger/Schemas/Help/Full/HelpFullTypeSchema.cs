namespace RiotGames.Help2Swagger;

public class HelpFullTypeSchema : HelpFullSchemaItemBase
{
    public HelpFullTypeFieldSchema[] Fields { get; set; }
    public int Size { get; set; }
    public HelpFullTypeValueSchema[] Values { get; set; }
}

public class HelpFullTypeValueSchema
{
    public string Description { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}

public class HelpFullTypeFieldSchema
{
    public string Description { get; set; }
    public string Name { get; set; }
    public int Offset { get; set; }
    public bool Optional { get; set; }
    public HelpFullSchemaTypeFieldTypeSchema SchemaType { get; set; }
}

public class HelpFullSchemaTypeFieldTypeSchema : HelpFullSchemaTypeReferenceBase
{
}