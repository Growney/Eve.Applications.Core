using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Http;

namespace Eve.EveAuthTool.Standard.Helpers
{
    public class HttpContextScopeParameters : IScopeParameters
    {
        public Tenant CurrentTenant
        {
            get
            {
                return m_storage.Tenant;
            }
        }
        public ICommandController TenantController
        {
            get
            {
                return m_controllerProvider.GetController();
            }
        }
        public IAllowedCharactersProvider Characters { get; }
        public bool IsAuthenticated {
            get
            {
                return m_context.HttpContext.User.Identity.IsAuthenticated;
            }
        }
        public long? MainCharacterID
        {
            get
            {
                return User?.GetMainCharacterID(TenantController);
            }
        }
        private readonly OnceLoadedValue<UserAccount> m_user = new OnceLoadedValue<UserAccount>();
        public UserAccount User
        {
            get
            {
                m_user.Load  = () => UserAccount.ForGuid(TenantController, m_context.HttpContext.User.Identity.Name);
                return m_user.Value;
            }
        }
        private readonly OnceLoadedValue<Task<Role>> m_currentRole = new OnceLoadedValue<Task<Role>>();
        public Task<Role> CurrentRole
        {
            get
            {
                m_currentRole.Load = () => GetCurrentRole();
                return m_currentRole.Value;
            }
        }

        public bool IsTenant
        {
            get
            {
                return CurrentTenant != null;
            }
        }

        //the reason we store all the providers instead of calculating from the constructor is that the tenant is not calculated when the object is created.
        private readonly ISingleParameters m_singles;
        private readonly ITenantStorage m_storage;
        private readonly ITenantControllerProvider m_controllerProvider;
        private readonly IHttpContextAccessor m_context;

        public HttpContextScopeParameters(ISingleParameters singles,IHttpContextAccessor context,ITenantStorage storage,ITenantControllerProvider controllerProvider,IAllowedCharactersProvider characterProvider)
        {
            m_singles = singles;
            m_storage = storage;
            m_controllerProvider = controllerProvider;
            m_context = context;

            Characters = characterProvider;        
        }

        private async Task<Role> GetCurrentRole()
        {
            Role retVal = null;
            List<AuthenticatedEntity> characters = AuthenticatedEntity.GetForAccount(m_singles.ESIConfiguration, TenantController, m_singles.Cache, m_singles.PublicDataProvider, User);
            foreach (AuthenticatedEntity character in characters)
            {
                retVal = await AuthRule.GetEntityRole(m_singles.ESIConfiguration, TenantController, m_singles.Cache, m_singles.PublicDataProvider, character);
                if (retVal != null)
                {
                    break;
                }
            }
            return retVal;
        }

    }
}
