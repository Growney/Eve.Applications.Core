using Eve.EveAuthTool.Standard.Discord.Configuration;
using Microsoft.Extensions.Configuration;

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
        public DiscordBotConfiguration(IConfiguration configuration)
        {
            BotKey = configuration["Discord:BotKey"].ToString();
            CommandPrefix = configuration["Discord:CommandPrefix"].ToString();
            AuthUrl = configuration["Discord:AuthUrl"].ToString();
            ClientID = configuration["Discord:AuthClientID"].ToString();
            CallBackUrl = configuration["Discord:CallbackURL"].ToString();
            Secret = configuration["Discord:Secret"].ToString();
        }
    }
}
