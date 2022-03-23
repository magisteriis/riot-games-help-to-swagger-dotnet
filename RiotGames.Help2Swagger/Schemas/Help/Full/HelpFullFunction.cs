namespace RiotGames.Help2Swagger;

public class HelpFullFunction : HelpFullItemBase
{
    public HelpFullFunctionArgument[] Arguments { get; set; }
    public string Async { get; set; }
    public string Help { get; set; }
    public HelpFullFunctionReturn Returns { get; set; }
    public bool ThreadSafe { get; set; }
}

public class HelpFullFunctionReturn : HelpFullTypeReferenceBase
{
}

public class HelpFullFunctionArgument
{
    public string Description { get; set; }
    public string Name { get; set; }
    public bool Optional { get; set; }
    public HelpFullFunctionArgumentType Type { get; set; }
}

public class HelpFullFunctionArgumentType : HelpFullTypeReferenceBase
{
}