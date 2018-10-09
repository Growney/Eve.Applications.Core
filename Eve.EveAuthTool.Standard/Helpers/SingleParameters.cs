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

namespace Eve.EveAuthTool.Standard.Helpers
{
    public class SingleParameters : ISingleParameters
    {
        public IStaticDataCache Cache { get; }
        public IESIAuthenticatedConfig ESIConfiguration { get; }
        public IPublicDataProvider PublicDataProvider { get; }
        public ITenantWebConfiguration TenantConfiguration { get; }

        public SingleParameters(IStaticDataCache cache, IESIAuthenticatedConfig esiConfiguration, IPublicDataProvider publicData, ITenantWebConfiguration tenantconfiguration)
        {
            Cache = cache;
            ESIConfiguration = esiConfiguration;
            PublicDataProvider = publicData;
            TenantConfiguration = tenantconfiguration;
        }
    }
}
