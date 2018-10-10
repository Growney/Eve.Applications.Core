using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Eve.ESI.Standard.Account;
using Eve.EveAuthTool.GUI.Web.Models.Discord;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Eve.EveAuthTool.Standard.Discord.Service;
using Eve.EveAuthTool.Standard.Discord.Service.Module;
using Eve.EveAuthTool.Standard.Discord.Service.Providers;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Web.OAuth;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eve.EveAuthTool.GUI.Web.Controllers
{
    [Authorize]
    [TenantRequired]
    public class DiscordController : Helpers.EveAuthBaseController<DiscordController>
    {
        private readonly OnceLoadedValue<string> m_linkString = new OnceLoadedValue<string>();
  
        public ulong? LinkedGuildID
        {
            get
            {
                ulong? retVal = null;
                if(ulong.TryParse(LinkedGuildString,out ulong id))
                {
                    retVal = id;
                }
                return retVal;
            }
        }
        public string LinkedGuildString
        {
            get
            {
                m_linkString.Load = () => TenantConfiguration.GetLink(CurrentTenant.Id, (byte)eTenantLinkType.Discord);
                return m_linkString.Value;
            }
        }
        public bool IsGuildLinked
        {
            get
            {
                return LinkedGuildID.HasValue ;
            }
        }
        
        public IArgumentsStore<DiscordLinkParameter> DiscordLinkStore { get; }
        public IDiscordBotConfiguration DiscordConfiguration { get; }
        public IArgumentsStore<OAuthRequestArguments> OAuthArgStore { get; }
        public IDiscordBot DiscordBot { get; }

        public DiscordController(IDiscordBot bot,ILogger<DiscordController> logger,IArgumentsStore<OAuthRequestArguments> oauthArgsStore,IDiscordBotConfiguration discordConfiguration,IArgumentsStore<DiscordLinkParameter> discordLinkStore, 
            ISingleParameters singles,IScopeParameters scopes)
            :base(logger,singles,scopes)
        {
            DiscordBot = bot;
            DiscordLinkStore = discordLinkStore;
            DiscordConfiguration = discordConfiguration;
            OAuthArgStore = oauthArgsStore;
        }
        private List<Models.Discord.DiscordRole> GetAllRoles(IGuild currentGuild)
        {
            List<Models.Discord.DiscordRole> allRoles = new List<Models.Discord.DiscordRole>();
            
            foreach (IRole role in currentGuild.Roles)
            {
                allRoles.Add(new Models.Discord.DiscordRole()
                {
                    ID = role.Id,
                    Name = role.Name
                });
            }
            return allRoles;
        }
        private async Task<List<Models.Discord.DiscordInvite>> GetInvites(IGuild currentGuild)
        {
            List<Models.Discord.DiscordInvite> allRoles = new List<Models.Discord.DiscordInvite>();
            IReadOnlyCollection<IInviteMetadata> invites = await currentGuild.GetInvitesAsync();
            foreach (IInviteMetadata invite in invites)
            {
                if (!invite.IsRevoked && !invite.IsTemporary)
                {
                    allRoles.Add(new Models.Discord.DiscordInvite()
                    {
                        ID = invite.Id,
                        Url = invite.Url
                    });
                }
                
            }
            return allRoles;
        }
        [Authorize(Roles ="Manage")]
        public async Task<IActionResult> Index()
        {
            if (IsGuildLinked)
            {
                IGuild currentGuild = await DiscordBot.GetGuild(LinkedGuildID.Value);
                return View("LinkedIndex", new Models.Discord.LinkedInfo()
                {
                    GuildName = currentGuild.Name,
                    Configurations = Models.Discord.DiscordConfiguration.CreateFrom<Models.Discord.DiscordConfiguration>(DiscordRoleConfiguration.All(TenantController), GetAllRoles(currentGuild),await GetInvites(currentGuild))
                });

            }
            else
            {
                return View("UnlinkedIndex", new Models.Discord.UnlinkedInfo()
                {
                    AddBotUrl = DiscordConfiguration.GetAddBotUrl(),
                    CommandPrefix = DiscordConfiguration.CommandPrefix
                });
            }
            
        }
        [HttpGet]
        [Authorize(Roles = "Manage")]
        public async Task<IActionResult> EditConfiguration(long configID)
        {
            DiscordRoleConfiguration config = DiscordRoleConfiguration.Get<DiscordRoleConfiguration>(TenantController, configID);
            if(config == null)
            {
                config = new DiscordRoleConfiguration()
                {
                    Name = "New Discord Configuration"
                };
            }
            IGuild currentGuild = await DiscordBot.GetGuild(LinkedGuildID.Value); ;

            List<Models.Discord.DiscordInvite> invites = await GetInvites(currentGuild);
            Models.Discord.EditDiscordConfiguration configuration = Models.Discord.DiscordConfiguration.CreateFrom<Models.Discord.EditDiscordConfiguration>(config, GetAllRoles(currentGuild), invites);
            configuration.Invites = invites;
            return View(configuration);
        }
        [HttpPost]
        [Authorize(Roles = "Manage")]
        public IActionResult EditConfiguration(Models.Discord.DiscordConfiguration config)
        {
            DiscordRoleConfiguration roleConfig = config.Create();
            roleConfig.Save(TenantController);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Manage")]
        public IActionResult GenerateLink()
        {
            return new JsonResult(DiscordLinkStore.StoreArguments(new DiscordLinkParameter(CurrentTenant.Id)));
        }
        public async Task<IUser> GetDiscordUser(string token)
        {
            DiscordRestClient client = CreateDiscordClient();
            await client.LoginAsync(TokenType.Bearer, token);
            IUser retVal = client.CurrentUser;
            await client.LogoutAsync();
            return retVal;
        }
        private DiscordRestClient CreateDiscordClient()
        {
            DiscordRestClient client = new DiscordRestClient(new DiscordRestConfig()
            {
                LogLevel = LogSeverity.Info
            });
            client.Log += x =>
            {
                if (x.Severity == LogSeverity.Error)
                {
                    Logger.LogError(x.Exception, "Discord Rest client exception");
                }
                else
                {
                    Logger.LogInformation(x.Message);
                }

                return Task.CompletedTask;
            };
            return client;
        }
        [Authorize]
        public async Task<IActionResult> LinkDiscordAccount([FromServices]IDiscordLinkProvider linkProvider,string state)
        {
            DiscordOAuthRequestArguments arguments = OAuthArgStore.ReCallArguments(state) as DiscordOAuthRequestArguments;
            try
            {
                if (arguments != null)
                {
                    if (IsGuildLinked)
                    {
                        Standard.Discord.Authentication.AuthenticationToken token = await Standard.Discord.Authentication.AuthenticationToken.RequestToken(DiscordConfiguration, arguments.Code);
                        if (token != null)
                        {
                            DiscordRestClient client = CreateDiscordClient();
                            try
                            {
                                await client.LoginAsync(TokenType.Bearer, token.Access_Token);
                                if (client.CurrentUser != null)
                                {
                                    if (linkProvider != null)
                                    {
                                        LinkedUserAccount currentLink = LinkedUserAccount.ForLink(TenantController, client.CurrentUser.Id.ToString());
                                        if (currentLink == null || currentLink.Id == CurrentAccount.Id)
                                        {
                                            LinkedUserAccount account = LinkedUserAccount.CreateLink(TenantController, CurrentAccount, (byte)eTenantLinkType.Discord, client.CurrentUser.Id.ToString());
                                            IGuild currentGuild = await DiscordBot.GetGuild(LinkedGuildID.Value); ;
                                            DiscordRoleConfiguration discordConfig = await CurrentDiscordConfiguration;
                                            IGuildUser guildUser = await linkProvider.GetAccountGuildUser(currentGuild, account);
                                            if (guildUser == null)
                                            {
                                                if (!string.IsNullOrEmpty(discordConfig?.GuildInviteID))
                                                {
                                                    await linkProvider.JoinUserToGuild(client, discordConfig.GuildInviteID);
                                                    guildUser = await linkProvider.GetAccountGuildUser(currentGuild, account,eCacheOptions.ForceReload);
                                                }
                                                else
                                                {
                                                    return View("Error", new Models.Shared.ErrorModel() { Message = "You are not a member of this guild and the configuration does not contain an invite." });
                                                }
                                            }
                                            if (guildUser != null)
                                            {
                                                IGuildUser botGuildUser = await DiscordBot.GetBotGuildUser(LinkedGuildID.Value);
                                                if (linkProvider.CanActionUpdateOnUser(currentGuild, botGuildUser, guildUser))
                                                {
                                                    if (MainCharacterID.HasValue)
                                                    {
                                                        (bool updatedNickname, string newNickname) = await linkProvider.UpdateNickname(guildUser, MainCharacterID.Value);
                                                        (int addedCount, int removedCount, int shouldAdd, int shouldRemove) = await linkProvider.UpdateRoles(currentGuild.Roles, botGuildUser, guildUser, discordConfig);
                                                        return View("/Views/Discord/AuthSuccess.cshtml", new DiscordAuthSuccessResults()
                                                        {
                                                            UpdatedNickname = updatedNickname,
                                                            UpdatedNicknameTo = newNickname,
                                                            RolesAdded = addedCount,
                                                            RolesRemoved = removedCount,
                                                            RolesShouldHaveBeenAdded = shouldAdd,
                                                            RolesShouldHaveBeenRemoved = shouldRemove
                                                        });
                                                    }
                                                    else
                                                    {
                                                        return View("Error", new Models.Shared.ErrorModel() { Message = "We are unable to update your discord settings as you have no main character ID. We have linked your account to ESA anyway." });
                                                    }
                                                }
                                                else
                                                {
                                                    return View("Error", new Models.Shared.ErrorModel() { Message = "We are unable to update your discord settings as your permissions are greater than the bots. We have linked your account to ESA anyway." });
                                                }
                                            }
                                            else
                                            {
                                                return View("Error", new Models.Shared.ErrorModel() { Message = "The configuration invite failed to make you a member of the guild please join the guild manually" });
                                            }
                                        }
                                        else
                                        {
                                            return View("Error", new Models.Shared.ErrorModel() { Message = "This discord account is already linked to another user account" });
                                        }

                                    }
                                    else
                                    {
                                        return View("Error", new Models.Shared.ErrorModel() { Message = "Error creating link provider" });
                                    }
                                }
                                else
                                {
                                    return View("Error", new Models.Shared.ErrorModel() { Message = "Unable to login to discord account" });
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                await client.LogoutAsync();
                            }
                        }
                        else
                        {
                            return View("Error", new Models.Shared.ErrorModel() { Message = "Could not get discord token" });
                        }

                    }
                    else
                    {
                        return View("Error", new Models.Shared.ErrorModel() { Message = "No Discord link" });
                    }
                    
                }
                else
                {
                    
                    return View("Error", new Models.Shared.ErrorModel() { Message = "Bad Discord Authorisation" });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Linking discord account");
                return View("Error", new Models.Shared.ErrorModel() { Message = ex.Message });
            }
            
        }
    }
}