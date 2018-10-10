using Discord;
using Discord.Rest;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Gware.Standard.Collections.Generic;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DiscordBot> m_logger;

        private readonly ICache<ulong, Dictionary<ulong,IGuildUser>> m_guildMembersCache;
        private readonly ICache<ulong, IGuild> m_guildCache;
        private readonly ICache<ulong, IUser> m_userCache;

        public DiscordBot(ILogger<DiscordBot> logger, IDiscordBotConfiguration configuration,
            ICache<ulong, Dictionary<ulong, IGuildUser>> guildMembersCache,
            ICache<ulong, IGuild> guildCache,
            ICache<ulong, IUser> userCache)
        {
            m_logger = logger;
            m_config = configuration;

            m_guildMembersCache = guildMembersCache;
            m_guildMembersCache.KeepFor = TimeSpan.FromMinutes(10);
            m_guildMembersCache.Read = async x =>
            {
                await m_loginTask;
                IGuild guild = await m_guildCache.GetItem(x);
                IReadOnlyCollection<IGuildUser> users = await guild.GetUsersAsync();
                return users.Index( k=>
                {
                    return k.Id;
                });
            };

            m_guildCache = guildCache;
            m_guildCache.KeepFor = TimeSpan.FromMinutes(30);
            m_guildCache.Read = async x =>
            {
                await m_loginTask;
                return await m_client.GetGuildAsync(x);
            };

            m_userCache = userCache;
            m_userCache.KeepFor = TimeSpan.FromMinutes(10);
            m_userCache.Read = async x =>
            {
                await m_loginTask;
                return await m_client.GetUserAsync(x);
            };

            m_client = CreateDiscordClient();
                                   
            m_loginTask = m_client.LoginAsync(TokenType.Bot, m_config.BotKey);
        }

        public Task<IUser> GetUser(ulong id, eCacheOptions cacheOptions = eCacheOptions.Default)
        {
            return m_userCache.GetItem(id, cacheOptions);
        }

        public Task<IGuild> GetGuild(ulong id, eCacheOptions cacheOptions = eCacheOptions.Default)
        {
            m_logger.LogTrace($"Getting discord guild {id}");
            return m_guildCache.GetItem(id, cacheOptions);
        }

        public async Task<IUser> GetBotUser()
        {
            await m_loginTask;
            m_logger.LogTrace($"Getting discord bot user");
            return m_client.CurrentUser;
        }
        
        public async Task<IGuildUser> GetBotGuildUser(ulong guildID, eCacheOptions cacheOptions = eCacheOptions.Default)
        {
            m_logger.LogTrace($"Getting bot guild user on guild {guildID}");
            Dictionary<ulong, IGuildUser> cachedValues = await m_guildMembersCache.GetItem(guildID, cacheOptions);
            return cachedValues.Get(m_client.CurrentUser.Id);
        }
        public async Task<IGuildUser> GetGuildUser(ulong guildID,ulong id, eCacheOptions cacheOptions = eCacheOptions.Default)
        {
            m_logger.LogTrace($"Getting guild user {id} on guild {guildID}");
            Dictionary<ulong, IGuildUser> cachedValues = await m_guildMembersCache.GetItem(guildID, cacheOptions);
            return cachedValues.Get(id);
        }

        public void Dispose()
        {
            m_client.LogoutAsync();
            m_guildMembersCache.Dispose();
            m_guildCache.Dispose();
            m_userCache.Dispose();

        }


        private DiscordRestClient CreateDiscordClient()
        {
            DiscordRestClient client = new DiscordRestClient(new DiscordRestConfig()
            {
                LogLevel = LogSeverity.Debug
            });
            client.Log += x =>
            {
                if (x.Severity == LogSeverity.Error)
                {
                    m_logger.LogError(x.Exception, "Discord Rest client exception");
                }
                else
                {
                    m_logger.LogInformation(x.Message);
                }

                return Task.CompletedTask;
            };
            return client;
        }
    }
}
