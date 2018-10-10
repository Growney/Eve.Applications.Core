using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Security.Middleware;
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
using Microsoft.Extensions.Logging;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Eve.EveAuthTool.Standard.Helpers
{
    public class SingleParameters : ISingleParameters
    {
        public IStaticDataCache Cache { get; }
        public IESIAuthenticatedConfig ESIConfiguration { get; }
        public IPublicDataProvider PublicDataProvider { get; }
        public ITenantConfiguration TenantConfiguration { get; }
        public IServiceProvider ServiceProvider { get; }

        public SingleParameters(IServiceProvider serviceProvider,IStaticDataCache cache, IESIAuthenticatedConfig esiConfiguration, IPublicDataProvider publicData, ITenantConfiguration tenantconfiguration)
        {
            ServiceProvider = serviceProvider;
            Cache = cache;
            ESIConfiguration = esiConfiguration;
            PublicDataProvider = publicData;
            TenantConfiguration = tenantconfiguration;
        }
    }
}
