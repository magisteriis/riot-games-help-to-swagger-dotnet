using System.Text.Json.Serialization;

namespace RiotGames.Help;

public class HelpConsoleTypeSchema
{
    public string? Description { get; set; }
    public Dictionary<string, HelpConsoleTypeFieldSchema>[]? Fields { get; set; }
    public HelpConsoleTypeValueSchema[]? Values { get; set; }
}

public class HelpConsoleTypeValueSchema
{
    public string Description { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}

public class HelpConsoleTypeFieldSchema
{
    public string Description { get; set; }
    public int Offset { get; set; }
    public bool Optional { get; set; }

    /// <summary>
    ///     Either a string or a (string name, HelpConsoleTypeSchema type).
    /// </summary>
    [JsonConverter(typeof(HelpConsoleTypeReferenceConverter))]
    public object Type { get; set; }
}