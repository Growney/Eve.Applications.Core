using Discord;
using Discord.Commands;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service.Module
{
    public abstract class EveAuthToolModuleBase<T> : ModuleBase<ContextWithDI<SocketCommandContext>>
    {

        protected OnceLoadedValue<Task<HashSet<ulong>>> m_botGuildRoleIDs = new OnceLoadedValue<Task<HashSet<ulong>>>();
        public Task<HashSet<ulong>> BotGuildRoleIds
        {
            get
            {
                m_botGuildRoleIDs.Load = async () =>
                {
                    return GetUserRoleIds(await BotGuildUser);
                };
                return m_botGuildRoleIDs.Value;
            }
        }

        public static HashSet<ulong> GetUserRoleIds(IGuildUser user)
        {
            HashSet<ulong> retVal = new HashSet<ulong>();

            if (user != null)
            {
                foreach (ulong roleID in user.RoleIds)
                {
                    if (!retVal.Contains(roleID))
                    {
                        retVal.Add(roleID);
                    }
                }
            }
            

            return retVal;
        }
        protected OnceLoadedValue<Task<IEnumerable<IRole>>> m_botGuildRoles = new OnceLoadedValue<Task<IEnumerable<IRole>>>();
        public Task<IEnumerable<IRole>> BotGuildRoles
        {
            get
            {
                m_botGuildRoles.Load = async () =>
                {
                    IGuildUser botUser = await BotGuildUser;
                    return GetUserRoles(botUser.Guild,botUser);
                };
                return m_botGuildRoles.Value;
            }
        }
        public static IEnumerable<IRole> GetUserRoles(IGuild guild,IGuildUser user)
        {
            List<IRole> roles = new List<IRole>();
            HashSet<ulong> userGuildRoles = GetUserRoleIds(user);

            foreach (var role in guild.Roles)
            {
                if (userGuildRoles.Contains(role.Id))
                {
                    roles.Add(role);
                }
            }
            
            return roles;
        }


        protected OnceLoadedValue<Task<int>> m_botGuildHighestRole = new OnceLoadedValue<Task<int>>();
        public Task<int> BotGuildHighestRole
        {
            get
            {
                m_botGuildHighestRole.Load = async () =>
                {
                    IGuildUser botUser = await BotGuildUser;
                    return GetUserHighestRole(botUser.Guild, botUser);
                };
                return m_botGuildHighestRole.Value;
            }
        }
        public static int GetUserHighestRole(IGuild guild,IGuildUser user)
        {
            int max = int.MinValue;

            foreach (var role in GetUserRoles(guild,user))
            {
                max = Math.Max(role.Position, max);
            }

            return max;
        }
        protected OnceLoadedValue<Task<IGuildUser>> m_botGuildUser = new OnceLoadedValue<Task<IGuildUser>>();
        public Task<IGuildUser> BotGuildUser
        {
            get
            {
                m_botGuildUser.Load = () => Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id);
                return m_botGuildUser.Value;
            }
        }
        protected OnceLoadedValue<IESIAuthenticatedConfig> m_ESIConfig = new OnceLoadedValue<IESIAuthenticatedConfig>();
        public IESIAuthenticatedConfig ESIConfig
        {
            get
            {
                m_ESIConfig.Load = () => Context.Provider.GetService(typeof(IESIAuthenticatedConfig)) as IESIAuthenticatedConfig;
                return m_ESIConfig.Value;
            }
        }
        protected OnceLoadedValue<IStaticDataCache> m_cache = new OnceLoadedValue<IStaticDataCache>();
        public IStaticDataCache Cache
        {
            get
            {
                m_cache.Load = () => Context.Provider.GetService(typeof(IStaticDataCache)) as IStaticDataCache;
                return m_cache.Value;
            }
        }
        protected OnceLoadedValue<ILogger<T>> m_logger = new OnceLoadedValue<ILogger<T>>();
        public ILogger<T> Logger
        {
            get
            {
                m_logger.Load = () => Context.Provider.GetService(typeof(ILogger<T>)) as ILogger<T>;
                return m_logger.Value;
            }
        }

        protected OnceLoadedValue<ITenantConfiguration> m_tenantConfiguration = new OnceLoadedValue<ITenantConfiguration>();
        public ITenantConfiguration TenantConfiguration
        {
            get
            {
                m_tenantConfiguration.Load = () => Context.Provider.GetService(typeof(ITenantConfiguration)) as ITenantConfiguration;
                return m_tenantConfiguration.Value;
            }
        }
        public ICommandController PublicDataController
        {
            get
            {
                return Cache.Controller;
            }
        }
        protected OnceLoadedValue<IPublicDataProvider> m_publicDataProvider = new OnceLoadedValue<IPublicDataProvider>();
        public IPublicDataProvider PublicDataProvider
        {
            get
            {
                m_publicDataProvider.Load = () => Context.Provider.GetService(typeof(IPublicDataProvider)) as IPublicDataProvider;
                return m_publicDataProvider.Value;
            }
        }
        protected OnceLoadedValue<Tenant> m_currentTenant = new OnceLoadedValue<Tenant>();
        public Tenant CurrentTenant
        {
            get
            {
                m_currentTenant.Load = () => TenantConfiguration?.GetTenantFromLink(Context.Guild.Id.ToString());
                return m_currentTenant.Value;
            }
        }
        protected OnceLoadedValue<ICommandController> m_tenantController = new OnceLoadedValue<ICommandController>();
        public ICommandController TenantController
        {
            get
            {
                m_tenantController.Load = () => TenantConfiguration?.GetTenantController(CurrentTenant);
                return m_tenantController.Value;
            }
        }
        
        protected OnceLoadedValue<UserAccount> m_currentAccount = new OnceLoadedValue<UserAccount>();
        public UserAccount CurrentAccount
        {
            get
            {
                m_currentAccount.Load = () =>
                {
                    UserAccount retVal = null;
                    if (CurrentTenant != null)
                    {
                        retVal = LinkedUserAccount.ForLink(TenantController, Context.User.Id.ToString());
                    }
                    return retVal;
                };
                return m_currentAccount.Value;
            }
        }
    }
}
