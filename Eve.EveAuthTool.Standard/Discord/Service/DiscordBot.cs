using Discord;
using Discord.Rest;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public class DiscordBot : IDiscordBot,IDisposable
    {
        private Task m_loginTask;
        private readonly IDiscordBotConfiguration m_config;
        private readonly DiscordRestClient m_client;

        public DiscordBot(IDiscordBotConfiguration configuration)
        {
            m_config = configuration;

            m_client = new DiscordRestClient(new DiscordRestConfig()
            {
                LogLevel = LogSeverity.Critical
            });

            m_loginTask = m_client.LoginAsync(TokenType.Bot, m_config.BotKey);
        }

        public async Task<IUser> GetUser(ulong id)
        {
            await m_loginTask;
            return await m_client.GetUserAsync(id);
        }

        public async Task<IGuild> GetGuild(ulong id)
        {
            await m_loginTask;
            return await m_client.GetGuildAsync(id); ;
        }

        public async Task<IUser> GetBotUser(IGuild guild)
        {
            await m_loginTask;
            return await m_client.GetUserAsync(m_client.CurrentUser.Id);
        }

        public async Task<IUser> GetBotUser(Task<IGuild> guild)
        {
            await m_loginTask;
            return await GetBotUser(await guild);
        }

        public async Task<IGuildUser> GetGuildUser(ulong guildID,ulong id)
        {
            await m_loginTask;
            return await m_client.GetGuildUserAsync(guildID, id); ;
        }

        public void Dispose()
        {
            m_client.LogoutAsync();
        }

        public async Task<IGuildUser> GetGuildUser(Task<IGuild> guild, Task<IUser> user)
        {
            await m_loginTask;
            return await m_client.GetGuildUserAsync((await guild).Id, (await user).Id);
        }
    }
}
