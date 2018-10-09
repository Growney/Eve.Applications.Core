using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Eve.ESI.Standard.Account;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public interface IDiscordLinkProvider
    {
        bool CanActionUpdateOnUser(IGuild guild, IGuildUser botUser, IGuildUser updateOn);
        Task<IGuildUser> GetAccountGuildUser(IGuild guild, LinkedUserAccount account);
        Task JoinUserToGuild(IDiscordClient client, string inviteID);
        Task RemoveRoles(IEnumerable<IRole> guildRoles, IGuildUser updateOn);
        Task UpdateNickname(IGuildUser updateOn, long characterID);
        Task UpdateRoles(IEnumerable<IRole> guildRoles, IGuildUser updateOn, DiscordRoleConfiguration configuration);
    }
}