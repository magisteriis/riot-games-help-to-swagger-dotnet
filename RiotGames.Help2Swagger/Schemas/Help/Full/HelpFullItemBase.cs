namespace RiotGames.Help2Swagger;

public abstract class HelpFullItemBase
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string NameSpace { get; set; }

    /// <summary>
    ///     E.g. "$builtin, Plugins, Plugin lol-login".
    /// </summary>
    public string[] Tags { get; set; }
}