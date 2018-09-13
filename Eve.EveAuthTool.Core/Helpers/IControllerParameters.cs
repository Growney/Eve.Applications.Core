using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.AspNetCore.Http;

namespace Eve.EveAuthTool.Core.Helpers
{
    public interface IControllerParameters
    {
        IStaticDataCache Cache { get; }
        IAllowedCharactersProvider Characters { get; }
        IESIAuthenticatedConfig ESIConfiguration { get; }
        ICommandController TenantController { get; }
        ICommandController PublicDataController { get; }
        ITenantControllerProvider TenantProvider { get; }
        ITenantConfiguration TenantConfiguration { get; }
        PublicDataProvider PublicData { get; }

        ViewParameterPackage CreateViewParameters(HttpContext context);
    }
}