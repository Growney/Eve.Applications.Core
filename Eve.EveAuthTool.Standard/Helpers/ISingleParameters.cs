using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.AspNetCore.Http;
using System;

namespace Eve.EveAuthTool.Standard.Helpers
{
    public interface ISingleParameters
    {
        IStaticDataCache Cache { get; }
        IESIAuthenticatedConfig ESIConfiguration { get; }
        IPublicDataProvider PublicDataProvider { get; }
        ITenantWebConfiguration TenantConfiguration { get; }
        IServiceProvider ServiceProvider { get; }
        
    }
}