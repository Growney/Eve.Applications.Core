using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.EveAuthTool.Standard.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Configuration;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;

namespace Eve.EveAuthTool.Standard.Helpers
{
    public abstract class ScopeParametersBase : IScopeParameters
    {
        public bool IsTenant
        {
            get
            {
                return CurrentTenant != null;
            }
        }
        public ITypeSafeConfigurationProvider<eUserSetting> UserConfiguration { get; protected set; }
        public virtual Tenant CurrentTenant { get; protected set; }
        public virtual ICommandController TenantController { get; protected set; }
        public virtual IAllowedCharactersProvider Characters { get; protected set; }
        public virtual UserAccount User { get; protected set; }
        public bool IsAuthenticated
        {
            get
            {
                return User != null;
            }
        }
        public long? MainCharacterID
        {
            get
            {
                if(TenantController != null)
                {
                    return User?.GetMainCharacterID(TenantController);
                }
                return null;
            }
        }
        

        private readonly OnceLoadedValue<Task<Role>> m_currentRole = new OnceLoadedValue<Task<Role>>();
        public Task<Role> CurrentRole
        {
            get
            {
                m_currentRole.Load = () => Role.ForAccount(m_singles.ESIConfiguration, TenantController, m_singles.Cache, m_singles.PublicDataProvider, User);
                return m_currentRole.Value;
            }
        }
        private readonly OnceLoadedValue<Task<DiscordRoleConfiguration>> m_currentDiscordConfiguration = new OnceLoadedValue<Task<DiscordRoleConfiguration>>();
        public Task<DiscordRoleConfiguration> CurrentDiscordConfiguration
        {
            get
            {
                m_currentDiscordConfiguration.Load = () => DiscordRoleConfiguration.ForAccount(CurrentRole, TenantController);
                return m_currentDiscordConfiguration.Value;
            }
        }
        private readonly ISingleParameters m_singles;

        public ScopeParametersBase(ISingleParameters singles, ITypeSafeConfigurationProvider<eUserSetting> userConfig)
        {
            m_singles = singles;
            UserConfiguration = userConfig;
        }
    }
}
