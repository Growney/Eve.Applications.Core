using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Gware.Standard.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public interface IDiscordLinkProvider
    {
        bool CanActionUpdateOnUser(IGuild guild, IGuildUser botUser, IGuildUser updateOn);
        Task<IGuildUser> GetAccountGuildUser(IGuild guild, LinkedUserAccount account, eCacheOptions cacheOptions = eCacheOptions.Default);
        Task JoinUserToGuild(IDiscordClient client, string inviteID);
        Task<(bool updated,string to)> UpdateNickname(IGuildUser updateOn, long characterID);
        Task<(int addedCount, int removedCount, int shouldAdd, int shouldRemove)> RemoveRoles(IEnumerable<IRole> guildRoles, IGuildUser botUser, IGuildUser updateOn);
        Task<(int addedCount, int removedCount, int shouldAdd, int shouldRemove)> UpdateRoles(IEnumerable<IRole> guildRoles, IGuildUser botUser, IGuildUser updateOn, DiscordRoleConfiguration configuration);
        bool ShouldUpdateGuildInfo();
        Task UpdateGuild(IGuild guild, string name, eESIEntityType type, long entityID);
    }
}