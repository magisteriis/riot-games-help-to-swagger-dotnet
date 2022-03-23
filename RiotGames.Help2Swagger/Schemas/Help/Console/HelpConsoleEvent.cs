namespace RiotGames.Help;

public class HelpConsoleEvent
{
    public string? Description { get; set; }
    public Dictionary<string, HelpConsoleType> Type { get; set; }
}