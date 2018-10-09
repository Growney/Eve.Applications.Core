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
namespace Eve.EveAuthTool.Standard.Discord.Service.Module
{
    public class RegistrationCommands : EveAuthToolModuleBase<RegistrationCommands>
    {
        
        [Command("Link")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Link([Remainder]string code)
        {
            string guild = Context.Guild.Id.ToString();
            if (CurrentTenant == null)
            {
                if (Context.Provider.GetService(typeof(IArgumentsStore<DiscordLinkParameter>)) is IArgumentsStore<DiscordLinkParameter> config)
                {
                    Tenant tenant = null;
                    long? tenantID = config.ReCallArguments(code)?.TenantID;
                    if (tenantID.HasValue)
                    {
                        tenant = TenantConfiguration.GetTenant(tenantID.Value);
                    }
                    if (tenant != null)
                    {
                        config.DiscardArguments(code);
                        TenantConfiguration.CreateTenantLink(tenant.Id, (byte)eTenantLinkType.Discord, guild);
                        m_currentTenant.Reset();
                        await Context.Channel.SendMessageAsync($"Link to {tenant.DisplayName} updating guild settings");
                        await UpdateDiscordGuild(CurrentTenant, ESIConfig, Context.Guild);
                        await Context.Channel.SendMessageAsync($"Linking users");
                        await UpdateDiscordGuildUsers(Logger,PublicDataProvider,TenantController,CurrentTenant.EntityId,CurrentTenant.EntityType,ESIConfig, Cache, Context.Guild, await BotGuildHighestRole);
                        await Context.Channel.SendMessageAsync($"Link complete");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync($"Invalid Code");
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"No Config");
                }
                return;
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Already linked to {CurrentTenant.DisplayName}");
            }
            
        }

        public static async Task UpdateDiscordGuildUsers(ILogger<object> logger, IPublicDataProvider publicData,ICommandController tenantController, long tenantEntityID, int tenantEntityType, IESIAuthenticatedConfig esiConfig, IStaticDataCache cache,IGuild guild, int highestRole)
        {
            foreach (LinkedUserAccount account in LinkedUserAccount.AllLinked(tenantController, (byte)eTenantLinkType.Discord))
            {
                await UpdateLinkedAccount(logger,esiConfig, publicData,tenantController, tenantEntityID,tenantEntityType, cache, guild, highestRole, account);
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

        public static async Task<bool> UpdateLinkedAccount(ILogger<object> logger,IESIAuthenticatedConfig esiconfig, IPublicDataProvider publicDataProvider,ICommandController tenantController,long tenantEntityID,int tenantEntityType,IStaticDataCache cache,IGuild guild,int modifyingHighestRole,LinkedUserAccount account)
        {
            bool retVal = false;
            if(ulong.TryParse(account.Link,out ulong userID))
            {
                logger.LogTrace($"Parsed Discord linked account link to user id {userID} for account {account.AccountGuid}");
                IGuildUser user = await guild.GetUserAsync(userID);
                if(user != null)
                {
                    if (userID != guild.OwnerId)
                    {
                        if (GetUserHighestRole(guild, user) <= modifyingHighestRole)
                        {
                            logger.LogTrace($"Non user guild owner with user id {userID} on guild {guild.Name} starting link");
                            List<AuthenticatedEntity> accountCharacters = AuthenticatedEntity.GetForAccount(esiconfig, tenantController, cache, publicDataProvider, account);
                            if (accountCharacters.Count > 0)
                            {
                                string setTo = await publicDataProvider.GetTaggedCharacterName(accountCharacters[0].EntityID);
                                if (!string.IsNullOrWhiteSpace(setTo) && user.Nickname != setTo)
                                {
                                    await user?.ModifyAsync(x =>
                                    {
                                        x.Nickname = setTo.ToString();
                                    });
                                }

                                logger.LogInformation($"Linked {account.AccountGuid} to discord {guild.Id} with nickname {setTo.ToString()} on {(eESIEntityType)tenantEntityType}({tenantEntityID})");

                                    retVal = true;

                                Role role = await AuthRule.GetEntityRole(esiconfig, tenantController, cache, publicDataProvider, accountCharacters[0]);


                                DiscordRoleConfiguration botConfiguration;

                                List<ulong> toRemove = new List<ulong>();
                                List<ulong> toAdd = new List<ulong>();
                                if (role != null && (botConfiguration = DiscordRoleConfiguration.Get<DiscordRoleConfiguration>(tenantController, role.DiscordRoleConfigurationID)) != null)
                                {
                                    HashSet<ulong> currentRoles = user.RoleIds.Index();
                                    foreach (ulong currentRole in currentRoles)
                                    {
                                        if (currentRole != guild.EveryoneRole.Id)
                                        {
                                            if (!botConfiguration.AssignedRoles.Contains(currentRole))
                                            {
                                                toRemove.Add(currentRole);
                                            }
                                        }
                                    }

                                    foreach (ulong configRole in botConfiguration.AssignedRoles)
                                    {
                                        if (!currentRoles.Contains(configRole))
                                        {
                                            toAdd.Add(configRole);
                                        }
                                    }
                                    if (toAdd.Count > 0)
                                    {
                                        await user.AddRolesAsync(CreateRoleList(botConfiguration.AssignedRoles, guild.Roles));
                                    }
                                    if (toRemove.Count > 0)
                                    {
                                        await user.RemoveRolesAsync(CreateRoleList(toRemove, guild.Roles));
                                    }
                                }
                                else
                                {
                                    foreach (ulong currentRoll in user.RoleIds)
                                    {
                                        if (currentRoll != guild.EveryoneRole.Id)
                                        {
                                            toRemove.Add(currentRoll);
                                        }
                                    }
                                    if (toRemove.Count > 0)
                                    {
                                        await user.RemoveRolesAsync(CreateRoleList(toRemove, guild.Roles));
                                    }
                                }
                                logger.LogInformation($"Linked {account.AccountGuid} to discord {guild.Id} with role {role?.Name ?? "NULL"}({role?.Id.ToString() ?? "NULL"}) causing +{toAdd.Count}/-{toRemove.Count} roles on {(eESIEntityType)tenantEntityType}({tenantEntityID})");
                            }
                        }
                        else
                        {
                            var privateChat = await user.GetOrCreateDMChannelAsync();
                            await privateChat.SendMessageAsync($"We are sorry can't automatically update you on {guild.Name} because you have higher roles than we do. You are still linked to the site but you will have to do your own details.");
                        }

                    }
                    else
                    {
                        var privateChat = await user.GetOrCreateDMChannelAsync();
                        await privateChat.SendMessageAsync($"We are sorry can't automatically update you on {guild.Name} because you are the owner of the guild and discord doesn't let us. You are still linked to the site but you will have to do your own details.");
                    }
                }
                else
                {
                    logger.LogError($"Failed to get guild user {userID} on guild {guild.Name}");
                }
                
            }
            else
            {
                logger.LogError($"Failed to parse link to ulong value {account.Link} for account {account.AccountGuid}");
            }
            return retVal;
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
            if (CurrentTenant != null)
            {
                TenantConfiguration.DeleteTenantLink(CurrentTenant.Id, (byte)eTenantLinkType.Discord);
                await Context.Channel.SendMessageAsync($"Unlinked from {CurrentTenant.DisplayName}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"No link found to remove");
            }
        }

        [Command("Url")]
        public async Task Url()
        {
            if (CurrentTenant != null)
            {
                await Context.Channel.SendMessageAsync($"ESA Url : {TenantConfiguration.GetTenantRedirect(CurrentTenant.Name,string.Empty)}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"No link found");
            }
        }

        [Command("Characters")]
        public async Task Characters()
        {
            if(CurrentTenant != null)
            {
                if(CurrentAccount != null)
                {
                    List<AuthenticatedEntity> characters = AuthenticatedEntity.GetForAccount(ESIConfig,TenantController, Cache, PublicDataProvider,CurrentAccount.AccountGuid.ToString());
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
