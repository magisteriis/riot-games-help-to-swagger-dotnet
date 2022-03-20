using System.Text.Json.Serialization;

namespace RiotGames.Help;


public class HelpConsoleFunctionArgument
{
    public string? Description { get; set; }

    /// <summary>
    /// Either a string or a (string name, HelpConsoleTypeSchema type).
    /// </summary>
    [JsonConverter(typeof(HelpConsoleTypeReferenceConverter))]
    public object Type { get; set; }
    public bool? Optional { get; set; }
}

public class HelpConsoleFunctionSchema
{
    public Dictionary<string, HelpConsoleFunctionArgument>[] Arguments { get; set; }
    
    public string Description { get; set; }

    [JsonPropertyName("http_method")]
    public string HttpMethod { get; set; }

    public int Privilege { get; set; }

    /// <summary>
    /// Either a string or a (string name, HelpConsoleTypeSchema type).
    /// </summary>
    [JsonConverter(typeof(HelpConsoleTypeReferenceConverter))]
    public object Returns { get; set; }

    public string Url { get; set; }

    public string Usage { get; set; }
}