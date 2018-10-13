using Eve.EveAuthTool.Standard.Configuration;
using Eve.EveAuthTool.Standard.Helpers;
using Gware.Standard.Configuration;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public class DiscordSyncScopeParameters : ScopeParametersBase
    {
        public DiscordSyncScopeParameters(ITypeSafeConfigurationProvider<eUserSetting> userConfig,ISingleParameters singles, ITenantStorage storage, ITenantControllerProvider controllerProvider)
            : base(singles,userConfig)
        {
            CurrentTenant = storage.Tenant;
            TenantController = controllerProvider?.GetController();
        }
    }
}
