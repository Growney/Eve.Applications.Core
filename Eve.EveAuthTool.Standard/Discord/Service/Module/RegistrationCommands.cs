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

namespace Eve.EveAuthTool.Standard.Discord.Service.Module
{
    public class RegistrationCommands : EveAuthToolModuleBase
    {
        
        [Command("Link")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Link([Remainder]string code)
        {
            string guild = Context.Guild.Id.ToString();
            if (CurrentTenant == null)
            {
                if (Context.Provider.GetService(typeof(IArgumentsStore<DiscordLinkParameters>)) is IArgumentsStore<DiscordLinkParameters> config)
                {
                    Tenant tenant = config.ReCallArguments(code)?.Tenant;
                    if (tenant != null)
                    {
                        config.DiscardArguments(code);
                        TenantConfiguration.CreateTenantLink(tenant.Id, (byte)eTenantLinkType.Discord, guild);
                        m_currentTenant.Reset();
                        await Context.Channel.SendMessageAsync($"Link to {tenant.DisplayName} updating guild settings");
                        await UpdateDiscordGuild(CurrentTenant, ESIConfig, Context.Guild);
                        await Context.Channel.SendMessageAsync($"Linking users");
                        await UpdateDiscordGuildUsers(PublicDataProvider,TenantController,CurrentTenant.EntityId,CurrentTenant.EntityType,ESIConfig, Cache, Context.Guild, await BotGuildHighestRole);
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

        public static async Task UpdateDiscordGuildUsers(PublicDataProvider publicData,ICommandController tenantController, long tenantEntityID, int tenantEntityType, IESIAuthenticatedConfig esiConfig, IStaticDataCache cache,IGuild guild, int highestRole)
        {
            foreach (LinkedUserAccount account in LinkedUserAccount.AllLinked(tenantController, (byte)eTenantLinkType.Discord))
            {
                await UpdateLinkedAccount(esiConfig, publicData,tenantController, tenantEntityID,tenantEntityType, cache, guild, highestRole, account);
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

        public static async Task<bool> UpdateLinkedAccount(IESIAuthenticatedConfig esiconfig,PublicDataProvider publicDataProvider,ICommandController tenantController,long tenantEntityID,int tenantEntityType,IStaticDataCache cache,IGuild guild,int modifyingHighestRole,LinkedUserAccount account)
        {
            bool retVal = false;
            if(ulong.TryParse(account.Link,out ulong userID))
            {
                IGuildUser user = await guild.GetUserAsync(userID);
                if (userID != guild.OwnerId)
                {
                    if (GetUserHighestRole(user) <= modifyingHighestRole)
                    {
                        List<AuthenticatedEntity> accountCharacters = AuthenticatedEntity.GetForAccount(esiconfig, tenantController, cache,publicDataProvider, account);
                        if (accountCharacters.Count > 0)
                        {
                            AuthenticatedEntity firstCharacter = accountCharacters[0];
                            ESICallResponse<CharacterInfo> character = await publicDataProvider.GetCharacterInfo(firstCharacter.EntityID,true);
                            if (character.HasData)
                            {
                                bool containsPrefix = false;
                                StringBuilder nickname = new StringBuilder();
                                if (character.Data.AllianceId > 0)
                                {
                                    if (tenantEntityType == (int)eESIEntityType.alliance && tenantEntityID != character.Data.AllianceId
                                        || tenantEntityType == (int)eESIEntityType.corporation && tenantEntityID != character.Data.CorporationId)
                                    {
                                        ESICallResponse<AllianceInfo> allianceInfo = await publicDataProvider.GetAllianceInfo(character.Data.AllianceId, true);
                                        if (allianceInfo.HasData)
                                        {
                                            nickname.Append($"[{allianceInfo.Data.Ticker}]");
                                            containsPrefix = true;
                                        }
                                    }
                                }
                                if (tenantEntityType == (int)eESIEntityType.alliance
                                    || tenantEntityType == (int)eESIEntityType.corporation && tenantEntityID != character.Data.CorporationId)
                                {
                                    ESICallResponse<CorporationInfo> corporationInfo = await publicDataProvider.GetCorporationInfo(character.Data.CorporationId, true);
                                    if (corporationInfo.HasData)
                                    {
                                        nickname.Append($"[{corporationInfo.Data.Ticker}]");
                                        containsPrefix = true;
                                    }
                                }

                                if (containsPrefix)
                                {
                                    nickname.Append(" ");
                                }
                                
                                nickname.Append(character.Data.Name);
                                await user?.ModifyAsync(x =>
                                {
                                    x.Nickname = nickname.ToString();
                                });
                                retVal = true;
                            }
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
            return retVal;
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
