namespace RiotGames.Help2Swagger;

public class HelpFullFunctionSchema : HelpFullSchemaItemBase
{
    public HelpFullFunctionArgumentSchema[] Arguments { get; set; }
    public string Async { get; set; }
    public string Help { get; set; }
    public HelpFullSchemaFunctionReturnSchema Returns { get; set; }
    public bool ThreadSafe { get; set; }
}

public class HelpFullSchemaFunctionReturnSchema : HelpFullSchemaTypeReferenceBase
{
}

public class HelpFullFunctionArgumentSchema
{
    public string Description { get; set; }
    public string Name { get; set; }
    public bool Optional { get; set; }
    public HelpFullSchemaFunctionArgumentTypeSchema Type { get; set; }
}

public class HelpFullSchemaFunctionArgumentTypeSchema : HelpFullSchemaTypeReferenceBase
{
}