using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;

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
        public UserAccount User { get; }

        public ViewParameterPackage(IStaticDataCache cache, IESIAuthenticatedConfig esiConfig, Tenant tenant, ICommandController tenantController, IAllowedCharactersProvider characters,UserAccount account)
        {
            TenantController = tenantController;
            CurrentTenant = tenant;
            ESIConfiguration = esiConfig;
            Cache = cache;
            Characters = characters;
            User =  account;
        }
    }
}
