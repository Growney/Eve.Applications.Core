using Discord;
using Gware.Standard.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public interface IDiscordBot
    {

        Task<IUser> GetBotUser();
        Task<IUser> GetUser(ulong id, eCacheOptions cacheOptions = eCacheOptions.Default);
        Task<IGuild> GetGuild(ulong id, eCacheOptions cacheOptions = eCacheOptions.Default);
        Task<IGuildUser> GetGuildUser(ulong guildID, ulong id, eCacheOptions cacheOptions = eCacheOptions.Default);
        Task<IGuildUser> GetBotGuildUser(ulong guildID, eCacheOptions cacheOptions = eCacheOptions.Default);
    }
}
