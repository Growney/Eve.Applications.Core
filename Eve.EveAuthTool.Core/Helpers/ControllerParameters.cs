using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Core.Helpers
{
    public class ControllerParameters : IControllerParameters
    {
        public IAllowedCharactersProvider Characters { get; }
        public IESIAuthenticatedConfig ESIConfiguration { get; }
        public IStaticDataCache Cache { get; }
        public ITenantControllerProvider TenantProvider { get; }
        public ICommandController TenantController
        {
            get
            {
                return TenantProvider.GetController();
            }
        }
        public ICommandController PublicDataController
        {
            get
            {
                return TenantProvider.GetDefaultDataController();
            }
        }
        public ITenantConfiguration TenantConfiguration { get; }

        public PublicDataProvider PublicData
        {
            get
            {
                return new PublicDataProvider(ESIConfiguration.Client, PublicDataController, Cache, TenantController);
            }
        }

        public ControllerParameters(ITenantConfiguration tenantConfiguration,ITenantControllerProvider tenantControllerProvider, IESIAuthenticatedConfig esiConfig, IAllowedCharactersProvider characters, IStaticDataCache cache)
        {
            TenantProvider = tenantControllerProvider;
            ESIConfiguration = esiConfig;
            Characters = characters;
            Cache = cache;
            TenantConfiguration = tenantConfiguration;
        }

        public ViewParameterPackage CreateViewParameters(HttpContext context)
        {
            UserAccount account = null;
            if (context.User.Identity.IsAuthenticated)
            {
                account = UserAccount.ForGuid(TenantController,context.User.Identity.Name);
            }
            return new ViewParameterPackage(Cache, ESIConfiguration, context.Features.Get<Tenant>(), TenantController,PublicData, Characters,account, TenantConfiguration);
        }
    }
}
