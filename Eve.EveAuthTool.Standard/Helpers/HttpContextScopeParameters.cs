using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.EveAuthTool.Standard.Configuration;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Configuration;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Http;

namespace Eve.EveAuthTool.Standard.Helpers
{
    public class HttpContextScopeParameters : ScopeParametersBase
    {
        public override Tenant CurrentTenant
        {
            get
            {
                return m_storage.Tenant;
            }
        }
        public override ICommandController TenantController
        {
            get
            {
                return m_controllerProvider.GetController();
            }
        }
        private readonly OnceLoadedValue<UserAccount> m_user = new OnceLoadedValue<UserAccount>();
        public override UserAccount User
        {
            get
            {
                m_user.Load  = () => UserAccount.ForGuid(TenantController, m_context.HttpContext.User.Identity.Name);
                return m_user.Value;
            }
        }
        //the reason we store all the providers instead of calculating from the constructor is that the tenant is not calculated when the object is created.
        private readonly ISingleParameters m_singles;
        private readonly ITenantStorage m_storage;
        private readonly ITenantControllerProvider m_controllerProvider;
        private readonly IHttpContextAccessor m_context;

        public HttpContextScopeParameters(ITypeSafeConfigurationProvider<eUserSetting> userConfig,ISingleParameters singles,IHttpContextAccessor context,ITenantStorage storage,ITenantControllerProvider controllerProvider,IAllowedCharactersProvider characterProvider)
            :base(singles,userConfig)
        {
            m_singles = singles;
            m_storage = storage;
            m_controllerProvider = controllerProvider;
            m_context = context;

            Characters = characterProvider;        
        }
    }
}
