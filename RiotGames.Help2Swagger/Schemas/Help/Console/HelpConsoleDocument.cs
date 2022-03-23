namespace RiotGames.Help;

public class HelpConsoleDocument
{
    /// <summary>
    ///     The Key is the name, e.g. "OnCallback".
    /// </summary>
    public Dictionary<string, HelpConsoleEvent> Events { get; set; }

    /// <summary>
    ///     The Key is the name, e.g. "DeleteLolChatV1ConversationsByIdMessages".
    /// </summary>
    public Dictionary<string, HelpConsoleFunction> Functions { get; set; }

    /// <summary>
    ///     The Key is the name, e.g. "ActiveBoostsLcdsStoreFulfillmentNotification".
    /// </summary>
    public Dictionary<string, HelpConsoleType> Types { get; set; }
}