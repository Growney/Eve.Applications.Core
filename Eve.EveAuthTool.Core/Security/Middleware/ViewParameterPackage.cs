using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;

namespace Eve.EveAuthTool.Core.Security.Middleware
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
        public ITenantConfiguration TenantConfiguration { get; }
        public PublicDataProvider PublicDataProvider { get; }

        public ViewParameterPackage(IStaticDataCache cache, IESIAuthenticatedConfig esiConfig, Tenant tenant, ICommandController tenantController,PublicDataProvider publicDataProvider, IAllowedCharactersProvider characters,UserAccount account,ITenantConfiguration tenantConfiguration)
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
