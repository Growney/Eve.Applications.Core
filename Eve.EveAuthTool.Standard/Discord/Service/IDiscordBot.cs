using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public interface IDiscordBot
    {

        Task<IUser> GetBotUser(Task<IGuild> guild);
        Task<IUser> GetBotUser(IGuild guild);
        Task<IUser> GetUser(ulong id);
        Task<IGuild> GetGuild(ulong id);
        Task<IGuildUser> GetGuildUser(ulong guildID, ulong id);
        Task<IGuildUser> GetGuildUser(Task<IGuild> guildID, Task<IUser> id);
    }
}
