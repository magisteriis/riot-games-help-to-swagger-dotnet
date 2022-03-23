using System.Text.Json.Serialization;

namespace RiotGames.Help;

public class HelpConsoleType
{
    public string? Description { get; set; }
    public Dictionary<string, HelpConsoleTypeField>[]? Fields { get; set; }
    public HelpConsoleTypeValue[]? Values { get; set; }
}

public class HelpConsoleTypeValue
{
    public string Description { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}

public class HelpConsoleTypeField
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