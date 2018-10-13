using Discord;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Eve.EveAuthTool.Standard.Helpers;
using Gware.Standard.Web;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public class DiscordSyncProvider : IDiscordSyncProvider
    {
        private readonly ISingleParameters m_singles;
        private readonly IScopeParameters m_scopes;
        private readonly IDiscordBot m_discordBot;
        private readonly IDiscordLinkProvider m_linkProvider;
        private readonly Tenant m_tenant;

        public DiscordSyncProvider(ITenantStorage tenantStorage,ISingleParameters singles,IScopeParameters scopes,IDiscordBot discordBot,IDiscordLinkProvider linkProvider)
        {
            m_singles = singles;
            m_scopes = scopes;
            m_discordBot = discordBot;
            m_linkProvider = linkProvider;
            m_tenant = tenantStorage.Tenant;
        }
        public async Task SyncDiscordTenant()
        {
            string tenantLink = m_singles.TenantConfiguration.GetLink(m_tenant.Id, (byte)eTenantLinkType.Discord);
            if (ulong.TryParse(tenantLink, out ulong guildID))
            {
                IGuild guild = await m_discordBot.GetGuild(guildID);
                if (guild != null)
                {
                    if (m_linkProvider.ShouldUpdateGuildInfo())
                    {
                        await m_linkProvider.UpdateGuild(guild, m_tenant.DisplayName, (eESIEntityType)m_tenant.EntityType, m_tenant.EntityId);
                    }
                    foreach (LinkedUserAccount account in LinkedUserAccount.AllLinked(m_scopes.TenantController, (byte)eTenantLinkType.Discord))
                    {
                        await UpdateAccountGuildUser(guild,account);
                    }
                }
            }
        }

        private async Task UpdateAccountGuildUser(IGuild guild,LinkedUserAccount account)
        {
            
            IGuildUser updateOn = await m_linkProvider.GetAccountGuildUser(guild, account);
            if (updateOn != null)
            {
                IGuildUser botUser = await m_discordBot.GetBotGuildUser(guild.Id);
                if (m_linkProvider.CanActionUpdateOnUser(guild, botUser, updateOn))
                {
                    DiscordRoleConfiguration discordRoleConfiguration = await DiscordRoleConfiguration.ForAccount(m_singles.ESIConfiguration, m_scopes.TenantController, m_singles.Cache, m_singles.PublicDataProvider, account);
                    long? characterID = account.GetMainCharacterID(m_scopes.TenantController);
                    if (characterID.HasValue)
                    {
                        await m_linkProvider.UpdateNickname(updateOn, characterID.Value);
                        await m_linkProvider.UpdateRoles(guild.Roles, botUser, updateOn, discordRoleConfiguration);
                    }
                }
            }
        }
    }
}
