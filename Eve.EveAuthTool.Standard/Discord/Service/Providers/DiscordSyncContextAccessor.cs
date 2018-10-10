using Eve.EveAuthTool.Standard.Helpers;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public class DiscordSyncScopeParameters : ScopeParametersBase
    {
        public DiscordSyncScopeParameters(ISingleParameters singles, ITenantStorage storage, ITenantControllerProvider controllerProvider)
            : base(singles)
        {
            CurrentTenant = storage.Tenant;
            TenantController = controllerProvider?.GetController();
        }
    }
}
