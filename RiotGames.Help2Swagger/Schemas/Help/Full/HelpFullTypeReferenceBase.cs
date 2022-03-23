namespace RiotGames.Help2Swagger;

public abstract class HelpFullTypeReferenceBase
{
    /// <summary>
    ///     E.g. "string".
    /// </summary>
    public string ElementType { get; set; }

    /// <summary>
    ///     E.g. "vector" or "string".
    /// </summary>
    public string Type { get; set; }
}