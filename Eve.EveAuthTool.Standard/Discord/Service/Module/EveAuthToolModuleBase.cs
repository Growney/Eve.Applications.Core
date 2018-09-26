using Discord;
using Discord.Commands;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service.Module
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
        protected OnceLoadedValue<IControllerParameters> m_controllerParameters = new OnceLoadedValue<IControllerParameters>();
        public IControllerParameters ControllerParameters
        {
            get
            {
                m_controllerParameters.Load = ()=> Context.Provider.GetService(typeof(IControllerParameters)) as IControllerParameters;
                return m_controllerParameters.Value;
            }
        }
        protected OnceLoadedValue<IESIAuthenticatedConfig> m_ESIConfig = new OnceLoadedValue<IESIAuthenticatedConfig>();
        public IESIAuthenticatedConfig ESIConfig
        {
            get
            {
                return ControllerParameters.ESIConfiguration;
            }
        }
        protected OnceLoadedValue<IStaticDataCache> m_cache = new OnceLoadedValue<IStaticDataCache>();
        public IStaticDataCache Cache
        {
            get
            {
                return ControllerParameters.Cache;
            }
        }
        protected OnceLoadedValue<ITenantConfiguration> m_tenantConfiguration = new OnceLoadedValue<ITenantConfiguration>();
        public ITenantConfiguration TenantConfiguration
        {
            get
            {
                return ControllerParameters.TenantConfiguration;
            }
        }
        protected OnceLoadedValue<ITenantControllerProvider> m_tenantControllerProvider = new OnceLoadedValue<ITenantControllerProvider>();
        public ITenantControllerProvider TenantControllerProvider
        {
            get
            {
                return ControllerParameters.TenantProvider;
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

        protected OnceLoadedValue<ICommandController> m_publicController = new OnceLoadedValue<ICommandController>();
        public ICommandController PublicDataController
        {
            get
            {
                m_publicController.Load = () => TenantControllerProvider.GetDefaultDataController();
                return m_publicController.Value;
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
