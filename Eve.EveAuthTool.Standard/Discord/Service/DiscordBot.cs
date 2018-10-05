using Discord;
using Discord.Rest;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public class DiscordBot : IDiscordBot
    {
        private readonly IDiscordBotConfiguration m_config;
        private readonly DiscordRestClient m_client;

        public DiscordBot(IDiscordBotConfiguration configuration)
        {
            m_config = configuration;

            m_client = new DiscordRestClient(new DiscordRestConfig()
            {
                LogLevel = LogSeverity.Critical
            });
        }

        public async Task<IUser> GetUser(ulong id)
        {
            await m_client.LoginAsync(TokenType.Bot, m_config.BotKey);

            IUser retVal = await m_client.GetUserAsync(id);

            await m_client.LogoutAsync();

            return retVal;
        }

        public async Task<IGuild> GetGuild(ulong id)
        {
            await m_client.LoginAsync(TokenType.Bot, m_config.BotKey);

            IGuild retVal = await m_client.GetGuildAsync(id);

            await m_client.LogoutAsync();

            return retVal;
        }

        public async Task<IUser> GetBotUser(IGuild guild)
        {
            await m_client.LoginAsync(TokenType.Bot, m_config.BotKey);

            IUser retVal = await m_client.GetUserAsync(m_client.CurrentUser.Id);

            await m_client.LogoutAsync();

            return retVal;
        }

        public async Task<IUser> GetBotUser(Task<IGuild> guild)
        {
            return await GetBotUser(await guild);
        }

        
    }
}
