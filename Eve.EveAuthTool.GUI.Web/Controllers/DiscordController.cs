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
        private readonly IDiscordBot m_bot;
        private readonly OnceLoadedValue<string> m_linkString = new OnceLoadedValue<string>();
        private readonly OnceLoadedValue<Task<IGuild>> m_currentGuild = new OnceLoadedValue<Task<IGuild>>();
        private readonly OnceLoadedValue<Task<IUser>> m_currentUser = new OnceLoadedValue<Task<IUser>>();
        private readonly OnceLoadedValue<Task<IUser>> m_botCurrentUser = new OnceLoadedValue<Task<IUser>>();

        
        public Task<IUser> BotCurrentUser
        {
            get
            {
                m_botCurrentUser.Load = () =>
                {
                    return m_bot.GetBotUser(CurrentGuild);
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
                        return m_bot.GetUser(userID);
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
                            return m_bot.GetGuild(guildID);
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

        private readonly IArgumentsStore<DiscordLinkParameter> m_discordLinkStore;
        private IDiscordBotConfiguration m_discordConfiguration;

        public DiscordController(IDiscordBot bot,IDiscordBotConfiguration discordConfiguration,IArgumentsStore<DiscordLinkParameter> discordLinkStore, IViewParameterProvider parameters)
            :base(parameters)
        {
            m_discordLinkStore = discordLinkStore;
            m_discordConfiguration = discordConfiguration;
            m_bot = bot;
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
        [Authorize(Roles ="Manage")]
        public async Task<IActionResult> Index()
        {
            if (IsGuildLinked)
            {
                IGuild currentGuild = await CurrentGuild;
                return View("LinkedIndex", new Models.Discord.LinkedInfo()
                {
                    GuildName = currentGuild.Name,
                    Configurations = Models.Discord.DiscordConfiguration.CreateFrom(DiscordRoleConfiguration.All(TenantController), GetAllRoles(currentGuild))
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
        public async Task<IActionResult> EditConfiguration(long configID)
        {
            return View(Models.Discord.DiscordConfiguration.CreateFrom(DiscordRoleConfiguration.Get< DiscordRoleConfiguration>(TenantController,configID), GetAllRoles(await CurrentGuild)));
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
            return new JsonResult(m_discordLinkStore.StoreArguments(new DiscordLinkParameter(CurrentTenant.Id)));
        }
    }
}