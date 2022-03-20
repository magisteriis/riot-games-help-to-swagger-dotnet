namespace RiotGames.Help2Swagger;

public class HelpFullEventSchema : HelpFullSchemaItemBase
{
    public HelpFullSchemaEventTypeSchema Type { get; set; }
}

public class HelpFullSchemaEventTypeSchema : HelpFullSchemaTypeReferenceBase
{
}