using Eve.ESI.Standard.Authentication.Configuration;
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

namespace Eve.EveAuthTool.Blazor.Server.Controllers
{
    public class ControllerParameters
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

        public ControllerParameters(ITenantControllerProvider tenantControllerProvider, IESIAuthenticatedConfig esiConfig, IAllowedCharactersProvider characters, IStaticDataCache cache)
        {
            TenantProvider = tenantControllerProvider;
            ESIConfiguration = esiConfig;
            Characters = characters;
            Cache = cache;
        }

        public ViewParameterPackage CreateViewParameters(HttpContext context)
        {
            return new ViewParameterPackage(Cache, ESIConfiguration, context.Features.Get<Tenant>(), TenantController, Characters);
        }
    }
}
