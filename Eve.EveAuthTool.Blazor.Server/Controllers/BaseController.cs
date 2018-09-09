using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eve.EveAuthTool.Blazor.Server.Controllers
{
    public class BaseController : Controller
    {
        private ICommandController m_tenantController;
        protected ICommandController TenantController
        {
            get
            {
                if (m_tenantController == null)
                {
                    m_tenantController = m_parameters.TenantController;
                }
                return m_tenantController;
            }
        }
        protected IAllowedCharactersProvider Characters
        {
            get
            {
                return m_parameters.Characters;
            }
        }
        protected IESIAuthenticatedConfig ESIConfiguration
        {
            get
            {
                return m_parameters.ESIConfiguration;
            }
        }
        protected IStaticDataCache Cache
        {
            get
            {
                return m_parameters.Cache;
            }
        }
        private ControllerParameters m_parameters;

        protected bool IsTenant
        {
            get
            {
                return CurrentTenant != null;
            }
        }

        protected Tenant CurrentTenant
        {
            get
            {
                return HttpContext.Features.Get<Tenant>();
            }
        }
        private OnceLoadedValue<UserAccount> m_userAccount = new OnceLoadedValue<UserAccount>();
        protected UserAccount CurrentAccount
        {
            get
            {
                m_userAccount.Load = () => UserAccount.ForGuid(TenantController, HttpContext.User.Identity.Name);
                return m_userAccount.Value;
            }
        }

        public BaseController(ControllerParameters parameters)
        {
            m_parameters = parameters;
        }

        public ViewParameterPackage GetViewParameters()
        {
            return new ViewParameterPackage(Cache, ESIConfiguration, CurrentTenant, TenantController, Characters);
        }

        public IActionResult RedirectToError(string message)
        {
            return Redirect($"/Error?message={message}");
        }
    }
}