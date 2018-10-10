using Gware.Standard.Web.OAuth;
using System;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public interface IDiscordBotConfiguration : IOAuthConfiguration
    {
        string BotKey { get; }
        string CommandPrefix { get; }
        string GetAddBotUrl();
        TimeSpan SyncCycleTime { get; }
    }
}
