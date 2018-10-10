using Eve.EveAuthTool.Standard.Discord.Configuration;
using Microsoft.Extensions.Configuration;
using System;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public class DiscordBotConfiguration : IDiscordBotConfiguration
    {
        public string BotKey { get; }
        public string CommandPrefix { get; }
        public string AuthUrl { get; }
        public string ClientID { get; }
        public string CallBackUrl { get; }
        public string Secret { get; }
        public string BotPermissions { get; }
        public TimeSpan SyncCycleTime { get; }

        public DiscordBotConfiguration(IConfiguration configuration)
        {
            BotKey = configuration["Discord:BotKey"].ToString();
            CommandPrefix = configuration["Discord:CommandPrefix"].ToString();
            AuthUrl = configuration["Discord:AuthUrl"].ToString();
            ClientID = configuration["Discord:AuthClientID"].ToString();
            CallBackUrl = configuration["Discord:CallbackURL"].ToString();
            Secret = configuration["Discord:Secret"].ToString();
            BotPermissions =  configuration["Discord:BotPermissions"].ToString();

            string syncCycleTime = configuration["Discord:SyncCycleTime"];
            if(int.TryParse(syncCycleTime,out int time))
            {
                SyncCycleTime = TimeSpan.FromMinutes(time);
            }
            else
            {
                SyncCycleTime = TimeSpan.FromHours(1);
            }
        }

        public string GetAddBotUrl()
        {
            return $"{AuthUrl}/authorize?client_id={ClientID}&scope=bot&permissions={BotPermissions}";
        }
    }
}
