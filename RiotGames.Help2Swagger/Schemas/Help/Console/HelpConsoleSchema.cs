namespace RiotGames.Help;

public class HelpConsoleSchema
{
    /// <summary>
    ///     The Key is the name, e.g. "OnCallback".
    /// </summary>
    public Dictionary<string, HelpConsoleEventSchema> Events { get; set; }

    /// <summary>
    ///     The Key is the name, e.g. "DeleteLolChatV1ConversationsByIdMessages".
    /// </summary>
    public Dictionary<string, HelpConsoleFunctionSchema> Functions { get; set; }

    /// <summary>
    ///     The Key is the name, e.g. "ActiveBoostsLcdsStoreFulfillmentNotification".
    /// </summary>
    public Dictionary<string, HelpConsoleTypeSchema> Types { get; set; }
}