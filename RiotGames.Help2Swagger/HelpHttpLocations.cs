namespace RiotGames.Help2Swagger;

internal class HelpHttpLocations
{
    public HelpHttpLocations(string helpFullUrl, string helpConsoleUrl)
    {
        HelpFullUrl = new Uri(helpFullUrl);
        HelpConsoleUrl = new Uri(helpConsoleUrl);
    }

    public Uri HelpFullUrl { get; set; }
    public Uri HelpConsoleUrl { get; set; }

    public static HelpHttpLocations Lcu => new("https://www.mingweisamuel.com/lcu-schema/lcu/help.json",
        "https://www.mingweisamuel.com/lcu-schema/lcu/help.console.json");

    public static HelpHttpLocations Rcs => new("https://www.mingweisamuel.com/lcu-schema/rcs/help.json",
        "https://www.mingweisamuel.com/lcu-schema/rcs/help.console.json");
}