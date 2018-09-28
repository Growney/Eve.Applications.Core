using Gware.Standard.Web.OAuth;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public interface IDiscordBotConfiguration : IOAuthConfiguration
    {
        string BotKey { get; }
        string CommandPrefix { get; }
        string GetAddBotUrl();
    }
}
