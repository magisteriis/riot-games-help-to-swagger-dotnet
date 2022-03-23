namespace RiotGames.Help2Swagger;

public class HelpFullType : HelpFullItemBase
{
    public HelpFullTypeField[] Fields { get; set; }
    public int Size { get; set; }
    public HelpFullTypeValue[] Values { get; set; }
}

public class HelpFullTypeValue
{
    public string Description { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}

public class HelpFullTypeField
{
    public string Description { get; set; }
    public string Name { get; set; }
    public int Offset { get; set; }
    public bool Optional { get; set; }
    public HelpFullSchemaTypeFieldType SchemaType { get; set; }
}

public class HelpFullSchemaTypeFieldType : HelpFullTypeReferenceBase
{
}