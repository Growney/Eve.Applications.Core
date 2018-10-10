using Discord.Commands;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gware.Standard.Web.Tenancy;
using Eve.ESI.Standard;
using Eve.Static.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.DataItem.Character;
using Eve.ESI.Standard.DataItem.Alliance;
using Eve.ESI.Standard.DataItem.Corporation;
using Gware.Standard.Collections.Generic;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard;
using Gware.Standard.Storage.Controller;
using Eve.ESI.Standard.Authentication.Client;
using Eve.EveAuthTool.Standard.Security.Rules;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Eve.EveAuthTool.Standard.Discord.Service.Module
{
    public class RegistrationCommands : EveAuthToolModuleBase<RegistrationCommands>
    {

        [Command("Refresh")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Refresh()
        {
            await Context.Channel.SendMessageAsync($"Refreshing guild settings");
            await UpdateDiscordGuild(ScopeParameters.CurrentTenant, SingleParameters.ESIConfiguration, Context.Guild);
            await Context.Channel.SendMessageAsync($"Refreshing users");
            await UpdateDiscordGuildUsers();
            await Context.Channel.SendMessageAsync($"Refresh complete");
        }
        [Command("Link")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Link([Remainder]string code)
        {
            string guild = Context.Guild.Id.ToString();
            if (ScopeParameters.CurrentTenant == null)
            {
                
                Tenant tenant = null;
                long? tenantID = DiscordLinkStore.ReCallArguments(code)?.TenantID;
                if (tenantID.HasValue)
                {
                    tenant = SingleParameters.TenantConfiguration.GetTenant(tenantID.Value);
                }
                if (tenant != null)
                {
                    DiscordLinkStore.DiscardArguments(code);
                    SingleParameters.TenantConfiguration.CreateTenantLink(tenant.Id, (byte)eTenantLinkType.Discord, guild);
                    //After we create the link we create a new scope to allow the process to update the tenant
                    using(IServiceScope scope = ScopeFactory.CreateScope())
                    {
                        DiscordBotService.ConfigureServiceScope(SingleParameters.TenantConfiguration, scope, Context);
                        Context.Provider = scope.ServiceProvider;
                        Tenant linkedTenant = SingleParameters.TenantConfiguration.GetTenantFromLink(guild);
                        await Context.Channel.SendMessageAsync($"Link to {tenant.DisplayName} updating guild settings");
                        await UpdateDiscordGuild(linkedTenant, SingleParameters.ESIConfiguration, Context.Guild);
                        await Context.Channel.SendMessageAsync($"Linking users");
                        await UpdateDiscordGuildUsers();
                        await Context.Channel.SendMessageAsync($"Link complete");
                    }
                        
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Invalid Code");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Already linked to {ScopeParameters.CurrentTenant.DisplayName}");
            }
        }
        private async Task UpdateAccountGuildUser(LinkedUserAccount account)
        {
            IGuildUser updateOn = await LinkProvider.GetAccountGuildUser(Context.Guild, account);
            if(updateOn != null)
            {
                IGuildUser botUser = await Bot.GetBotGuildUser(Context.Guild.Id);
                if (LinkProvider.CanActionUpdateOnUser(Context.Guild, botUser, updateOn))
                {
                    DiscordRoleConfiguration discordRoleConfiguration = await DiscordRoleConfiguration.ForAccount(SingleParameters.ESIConfiguration, ScopeParameters.TenantController, SingleParameters.Cache, SingleParameters.PublicDataProvider, account);
                    long? characterID = account.GetMainCharacterID(ScopeParameters.TenantController);
                    if(characterID.HasValue)
                    {
                        await LinkProvider.UpdateNickname(updateOn, characterID.Value);
                        await LinkProvider.UpdateRoles(Context.Guild.Roles, botUser, updateOn, discordRoleConfiguration);
                    }
                }
            }
            
        }
        private async Task UpdateDiscordGuildUsers()
        {
            foreach (LinkedUserAccount account in LinkedUserAccount.AllLinked(ScopeParameters.TenantController, (byte)eTenantLinkType.Discord))
            {
                await UpdateAccountGuildUser(account);
            }
        }

        public static async Task UpdateDiscordGuild(Tenant tenant,IESIAuthenticatedConfig esiConfig,IGuild guild)
        {
            Image? image = null;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                byte[] imageData = await client.DownloadDataTaskAsync(esiConfig.GetImageSource((eESIEntityType)tenant.EntityType, tenant.EntityId, 128));
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream(imageData))
                {
                    image = new Image(stream);
                    await guild.ModifyAsync(x =>
                    {
                        x.Name = new Optional<string>(tenant.DisplayName);
                        x.Icon = new Optional<Image?>(image);
                    });
                }
            }
            
        }

        private static IEnumerable<IRole> CreateRoleList(IEnumerable<ulong> selected,IEnumerable<IRole> allRoles)
        {
            HashSet<ulong> set = new HashSet<ulong>(selected);
            List<IRole> roles = new List<IRole>();
            foreach(IRole role in allRoles)
            {
                if (set.Contains(role.Id))
                {
                    roles.Add(role);
                }
            }
            return roles;
        }

        [Command("Unlink")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Unlink()
        {
            if (ScopeParameters.CurrentTenant != null)
            {
                SingleParameters.TenantConfiguration.DeleteTenantLink(ScopeParameters.CurrentTenant.Id, (byte)eTenantLinkType.Discord);
                await Context.Channel.SendMessageAsync($"Unlinked from {ScopeParameters.CurrentTenant.DisplayName}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"No link found to remove");
            }
        }

        [Command("Url")]
        public async Task Url()
        {
            if (ScopeParameters.CurrentTenant != null)
            {
                await Context.Channel.SendMessageAsync($"ESA Url : {SingleParameters.TenantConfiguration.GetTenantRedirect(ScopeParameters.CurrentTenant.Name,string.Empty)}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"No link found");
            }
        }

        [Command("Characters")]
        public async Task Characters()
        {
            if(ScopeParameters.CurrentTenant != null)
            {
                if(ScopeParameters.User != null)
                {
                    List<AuthenticatedEntity> characters = AuthenticatedEntity.GetForAccount(SingleParameters.ESIConfiguration, ScopeParameters.TenantController, SingleParameters.Cache, SingleParameters.PublicDataProvider, ScopeParameters.User.AccountGuid.ToString());
                    StringBuilder message = new StringBuilder();
                    foreach (var character in characters)
                    {
                        message.AppendLine(character.EntityID.ToString());
                    }
                    await Context.Channel.SendMessageAsync(message.ToString());
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"No Account link found");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"No Tenant link found");
            }
            

        }
    }
}
