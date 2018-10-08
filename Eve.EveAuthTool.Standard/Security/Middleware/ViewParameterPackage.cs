using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Security;
using Eve.EveAuthTool.Standard.Security.Rules;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Security.Middleware
{
    public class ViewParameterPackage
    {
        public ICommandController TenantController { get; }
        public bool IsTenant
        {
            get
            {
                return CurrentTenant != null;
            }
        }
        public Tenant CurrentTenant{ get; }
        public IESIAuthenticatedConfig ESIConfiguration { get; }
        public IStaticDataCache Cache { get; }
        public IAllowedCharactersProvider Characters { get; }
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
                return User?.GetMainCharacterID(TenantController);
            }
        }
        public UserAccount User { get; }
        public ITenantWebConfiguration TenantConfiguration { get; }
        public PublicDataProvider PublicDataProvider { get; }

        private readonly OnceLoadedValue<Task<Role>> m_currentRole = new OnceLoadedValue<Task<Role>>();
        public Task<Role> CurrentRole
        {
            get
            {
                m_currentRole.Load = async () =>
                {
                    Role retVal = null;
                    List<AuthenticatedEntity> characters = AuthenticatedEntity.GetForAccount(ESIConfiguration, TenantController, Cache, PublicDataProvider, User);
                    foreach (AuthenticatedEntity character in characters)
                    {
                        retVal = await AuthRule.GetEntityRole(ESIConfiguration, TenantController, Cache, PublicDataProvider, character);
                        if (retVal != null)
                        {
                            break;
                        }
                    }
                    return retVal;
                };
                return m_currentRole.Value;
            }
        }
        
        public ViewParameterPackage(IStaticDataCache cache, IESIAuthenticatedConfig esiConfig, Tenant tenant, ICommandController tenantController,PublicDataProvider publicDataProvider, IAllowedCharactersProvider characters,UserAccount account, ITenantWebConfiguration tenantConfiguration)
        {
            TenantController = tenantController;
            CurrentTenant = tenant;
            ESIConfiguration = esiConfig;
            Cache = cache;
            Characters = characters;
            User =  account;
            TenantConfiguration = tenantConfiguration;
            PublicDataProvider = publicDataProvider;
        }
    }
}
