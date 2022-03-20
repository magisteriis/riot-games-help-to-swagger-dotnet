namespace RiotGames.Help;

public class HelpConsoleEventSchema
{
    public string? Description { get; set; }
    public Dictionary<string, HelpConsoleTypeSchema> Type { get; set; }
}