using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public class DiscordCommandContextScopeParameters : ScopeParametersBase
    {
        public DiscordCommandContextScopeParameters(ISingleParameters singles,IDiscordCommandContextAccessor accessor,ITenantStorage storage, ITenantControllerProvider controllerProvider, IAllowedCharactersProvider characterProvider)
            :base(singles)
        {
            CurrentTenant = storage.Tenant;
            TenantController = controllerProvider.GetController();
            Characters = characterProvider;

            if(CurrentTenant != null && TenantController != null)
            {
                if (accessor?.CommandContext.User != null)
                {
                    LinkedUserAccount account = LinkedUserAccount.ForLink(TenantController, accessor.CommandContext.User.Id.ToString());
                    User = account;
                }
            }
        }

        
    }
}
