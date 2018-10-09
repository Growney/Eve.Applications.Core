using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Eve.ESI.Standard.Account;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Eve.EveAuthTool.Standard.Discord.Service;
using Eve.EveAuthTool.Standard.Discord.Service.Module;
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
        private readonly OnceLoadedValue<Task<IGuild>> m_currentGuild = new OnceLoadedValue<Task<IGuild>>();
        private readonly OnceLoadedValue<Task<IUser>> m_currentUser = new OnceLoadedValue<Task<IUser>>();
        private readonly OnceLoadedValue<Task<IUser>> m_botCurrentUser = new OnceLoadedValue<Task<IUser>>();
        private readonly OnceLoadedValue<Task<IGuildUser>> m_botGuildUser = new OnceLoadedValue<Task<IGuildUser>>();
        public Task<IGuildUser> BotGuildUser
        {
            get
            {
                m_botGuildUser.Load = () =>
                {
                    return Bot.GetGuildUser(CurrentGuild,BotCurrentUser);
                };
                return m_botGuildUser.Value;
            }
        }

        public Task<IUser> BotCurrentUser
        {
            get
            {
                m_botCurrentUser.Load = () =>
                {
                    return Bot.GetBotUser(CurrentGuild);
                };
                return m_botCurrentUser.Value;
            }
        }
        public Task<IUser> CurrentUser
        {
            get
            {
                m_currentUser.Load = () =>
                {
                    string accountLink = CurrentAccount?.GetLink(TenantController, (byte)eTenantLinkType.Discord);
                    if (ulong.TryParse(accountLink, out ulong userID))
                    {
                        return Bot.GetUser(userID);
                    }

                    return null;
                };

                return m_currentUser.Value;
            }
        }
        public Task<IGuild> CurrentGuild
        {
            get
            {
                m_currentGuild.Load = () =>
                {
                    if (IsGuildLinked)
                    {
                        if (ulong.TryParse(LinkedGuildId, out ulong guildID))
                        {
                            return Bot.GetGuild(guildID);
                        }
                    }
                    return null;
                };
                return m_currentGuild.Value;
            }
        }
        public string LinkedGuildId
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
                return !string.IsNullOrEmpty(LinkedGuildId);
            }
        }

        public IDiscordBot Bot { get; }
        public IArgumentsStore<DiscordLinkParameter> DiscordLinkStore { get; }
        public IDiscordBotConfiguration DiscordConfiguration { get; }
        public IArgumentsStore<OAuthRequestArguments> OAuthArgStore { get; }

        public DiscordController(ILogger<DiscordController> logger,IArgumentsStore<OAuthRequestArguments> oauthArgsStore,IDiscordBot bot,IDiscordBotConfiguration discordConfiguration,IArgumentsStore<DiscordLinkParameter> discordLinkStore, 
            ISingleParameters singles,IScopeParameters scopes)
            :base(logger,singles,scopes)
        {
            DiscordLinkStore = discordLinkStore;
            DiscordConfiguration = discordConfiguration;
            Bot = bot;
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
                IGuild currentGuild = await CurrentGuild;
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
            IGuild currentGuild = await CurrentGuild;

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
        [Authorize]
        public async Task<IActionResult> LinkDiscordAccount(string state)
        {
            DiscordOAuthRequestArguments arguments = OAuthArgStore.ReCallArguments(state) as DiscordOAuthRequestArguments;
            try
            {
                if (arguments != null)
                {
                    Standard.Discord.Authentication.AuthenticationToken token = await Standard.Discord.Authentication.AuthenticationToken.RequestToken(DiscordConfiguration, arguments.Code);
                    if (token != null)
                    {
                        Discord.Rest.DiscordRestClient client = new Discord.Rest.DiscordRestClient();
                        client.Log += x =>
                        {
                            if (x.Severity == LogSeverity.Error)
                            {
                                Logger.LogError(x.Exception, "Discord Client exception");
                            }
                            else
                            {
                                Logger.LogInformation(x.Message);
                            }
                            
                            return Task.CompletedTask;
                        };
                        await client.LoginAsync(Discord.TokenType.Bearer, token.Access_Token);
                        if(client.CurrentUser != null)
                        {
                            string link = client.CurrentUser.Id.ToString();
                            LinkedUserAccount.CreateLink(TenantController, CurrentAccount.AccountGuid, (byte)eTenantLinkType.Discord, link);
                            LinkedUserAccount account = LinkedUserAccount.CreateObject(link, (byte)eTenantLinkType.Discord, CurrentAccount);

                            await JoinCurrentGuild(client);

                            if(await RegistrationCommands.UpdateLinkedAccount(Logger,ESIConfiguration, PublicDataProvider, TenantController, CurrentTenant.EntityId, CurrentTenant.EntityType, Cache, await CurrentGuild, RegistrationCommands.GetUserHighestRole(await CurrentGuild, await BotGuildUser), account))
                            {
                                return View("/Views/Registration/AuthSuccess.cshtml");
                            }
                            else
                            {
                                return View("Error", new Models.Shared.ErrorModel() { Message = "Your account has been created on ESA but we failed to link it to discord"});
                            }
                        }
                        else
                        {
                            return View("Error", new Models.Shared.ErrorModel() { Message = "Unable to login to discord account" });
                        }
                    }
                    else
                    {
                        return View("Error", new Models.Shared.ErrorModel() { Message = "Could not get discord token" });
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

        private async Task<bool> IsInCurrentGuild(IDiscordClient client)
        {
            ulong currentGuild = (await CurrentGuild).Id;
            bool retVal = false;
            IReadOnlyCollection<IGuild> guilds = await client.GetGuildsAsync();
            foreach(IGuild guild in guilds)
            {
                if(guild.Id == currentGuild)
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        private async Task JoinCurrentGuild(IDiscordClient client)
        {
            Role currentRole = await CurrentRole;
            if (currentRole != null)
            {
                DiscordRoleConfiguration discordConfig = DiscordRoleConfiguration.Get<DiscordRoleConfiguration>(TenantController, currentRole.DiscordRoleConfigurationID);
                if (discordConfig != null)
                {
                    if (!string.IsNullOrEmpty(discordConfig.GuildInviteID))
                    {
                        IInvite invite = await client.GetInviteAsync(discordConfig.GuildInviteID);
                        if (invite != null)
                        {
                            await invite.AcceptAsync();
                        }
                    }
                }
            }
        }
    }
}