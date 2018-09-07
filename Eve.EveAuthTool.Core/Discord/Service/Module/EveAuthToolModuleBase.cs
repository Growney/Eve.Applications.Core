using Discord;
using Discord.Commands;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Core.Discord.Service.Module
{
    public abstract class EveAuthToolModuleBase : ModuleBase<ContextWithDI<SocketCommandContext>>
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

            foreach (ulong roleID in  user.RoleIds)
            {
                if (!retVal.Contains(roleID))
                {
                    retVal.Add(roleID);
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
                    return GetUserRoles(await BotGuildUser);
                };
                return m_botGuildRoles.Value;
            }
        }
        public static IEnumerable<IRole> GetUserRoles(IGuildUser user)
        {
            List<IRole> roles = new List<IRole>();
            HashSet<ulong> userGuildRoles = GetUserRoleIds(user);
            foreach (var role in user.Guild.Roles)
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
                    return GetUserHighestRole(await BotGuildUser);
                };
                return m_botGuildHighestRole.Value;
            }
        }
        public static int GetUserHighestRole(IGuildUser user)
        {
            int max = int.MinValue;

            foreach (var role in GetUserRoles(user))
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
        protected OnceLoadedValue<ITenantConfiguration> m_tenantConfiguration = new OnceLoadedValue<ITenantConfiguration>();
        public ITenantConfiguration TenantConfiguration
        {
            get
            {
                m_tenantConfiguration.Load = () => Context.Provider.GetService(typeof(ITenantConfiguration)) as ITenantConfiguration;
                return m_tenantConfiguration.Value;
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
                        retVal = LinkedUserAccount.ForLink(CurrentTenant.Controller, Context.User.Id.ToString());
                    }
                    return retVal;
                };
                return m_currentAccount.Value;
            }
        }
    }
}
