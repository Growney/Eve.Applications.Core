using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Eve.EveAuthTool.Standard.Discord.Service;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Gware.Standard.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eve.EveAuthTool.GUI.Web.Controllers
{
    [Authorize]
    public class DiscordController : Helpers.EveAuthBaseController
    {
        private readonly IServiceProvider m_serviceProvider;
        private readonly OnceLoadedValue<string> m_linkString = new OnceLoadedValue<string>();
        private readonly OnceLoadedValue<IGuild> m_currentGuild = new OnceLoadedValue<IGuild>();
        private readonly OnceLoadedValue<IUser> m_currentUser = new OnceLoadedValue<IUser>();
        private readonly OnceLoadedValue<Task<IGuildUser>> m_botCurrentUser = new OnceLoadedValue<Task<IGuildUser>>();
        private readonly OnceLoadedValue<DiscordBotService> m_discordService = new OnceLoadedValue<DiscordBotService>();

        public DiscordBotService DiscordService
        {
            get
            {
                m_discordService.Load = () =>
                {
                    IEnumerable<Microsoft.Extensions.Hosting.IHostedService> services = m_serviceProvider.GetService(typeof(IEnumerable<Microsoft.Extensions.Hosting.IHostedService>)) as IEnumerable<Microsoft.Extensions.Hosting.IHostedService>;
                    foreach (var service in services)
                    {
                        if (service is DiscordBotService retVal)
                        {
                            return retVal;
                        }
                    }
                    return null;
                };
      
                return m_discordService.Value;
            }
        }
        public Task<IGuildUser> BotCurrentUser
        {
            get
            {
                m_botCurrentUser.Load = () =>
                {
                    return DiscordService.GetBotGuildUser(CurrentGuild);
                };
                return m_botCurrentUser.Value;
            }
        }

        public IUser CurrentUser
        {
            get
            {
                m_currentUser.Load = () =>
                {
                    string accountLink = CurrentAccount?.GetLink(TenantController, (byte)eTenantLinkType.Discord);
                    if (ulong.TryParse(accountLink, out ulong userID))
                    {
                        return DiscordService?.GetUser(userID);
                    }

                    return null;
                };

                return m_currentUser.Value;
            }
        }
        public IGuild CurrentGuild
        {
            get
            {
                m_currentGuild.Load = () =>
                {
                    if (IsGuildLinked)
                    {
                        if (ulong.TryParse(LinkedGuildId, out ulong guildID))
                        {
                            return DiscordService?.GetGuild(guildID);
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

        private readonly IArgumentsStore<DiscordLinkParameters> m_discordLinkStore;
        private IDiscordBotConfiguration m_discordConfiguration;

        public DiscordController(IServiceProvider provider,IDiscordBotConfiguration discordConfiguration,IArgumentsStore<DiscordLinkParameters> discordLinkStore, IViewParameterProvider parameters)
            :base(parameters)
        {
            m_discordLinkStore = discordLinkStore;
            m_discordConfiguration = discordConfiguration;
            m_serviceProvider = provider;
        }
        private List<Models.Discord.DiscordRole> GetAllRoles()
        {
            List<Models.Discord.DiscordRole> allRoles = new List<Models.Discord.DiscordRole>();
            foreach (IRole role in CurrentGuild?.Roles)
            {
                allRoles.Add(new Models.Discord.DiscordRole()
                {
                    ID = role.Id,
                    Name = role.Name
                });
            }
            return allRoles;
        }
        [Authorize(Roles ="Manage")]
        public IActionResult Index()
        {
            if (IsGuildLinked)
            {
                
                return View("LinkedIndex", new Models.Discord.LinkedInfo()
                {
                    GuildName = CurrentGuild.Name,
                    Configurations = Models.Discord.DiscordConfiguration.CreateFrom(DiscordRoleConfiguration.All(TenantController), GetAllRoles())
                });

            }
            else
            {
                return View("UnlinkedIndex", new Models.Discord.UnlinkedInfo()
                {
                    AddBotUrl = m_discordConfiguration.GetAddBotUrl(),
                    CommandPrefix = m_discordConfiguration.CommandPrefix
                });
            }
            
        }
        [HttpGet]
        [Authorize(Roles = "Manage")]
        public IActionResult EditConfiguration(long configID)
        {
            return View(Models.Discord.DiscordConfiguration.CreateFrom(DiscordRoleConfiguration.Get< DiscordRoleConfiguration>(TenantController,configID), GetAllRoles()));
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
            return new JsonResult(m_discordLinkStore.StoreArguments(new DiscordLinkParameters(CurrentTenant)));
        }
    }
}