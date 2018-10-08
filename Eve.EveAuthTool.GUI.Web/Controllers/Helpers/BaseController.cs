using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.AspNetCore.Mvc;
using Eve.EveAuthTool.Standard.Security.Rules;
using Microsoft.Extensions.Logging;

namespace Eve.EveAuthTool.GUI.Web.Controllers.Helpers
{
    public class EveAuthBaseController<T> : Controller
    {
        private IViewParameterProvider m_viewParameters;

        public ViewParameterPackage ViewParameters
        {
            get
            {
                return m_viewParameters.Package;
            }
        }
        protected ICommandController TenantController
        {
            get
            {
                return ViewParameters.TenantController;
            }
        }
        protected PublicDataProvider PublicDataProvider
        {
            get
            {
                return ViewParameters.PublicDataProvider;
            }
        }
        protected IAllowedCharactersProvider Characters
        {
            get
            {
                return ViewParameters.Characters;
            }
        }
        protected IESIAuthenticatedConfig ESIConfiguration
        {
            get
            {
                return ViewParameters.ESIConfiguration;
            }
        }
        protected IStaticDataCache Cache
        {
            get
            {
                return ViewParameters.Cache;
            }
        }

        protected bool IsTenant
        {
            get
            {
                return ViewParameters.IsTenant;
            }
        }

        protected Tenant CurrentTenant
        {
            get
            {
                return ViewParameters.CurrentTenant;
            }
        }
        protected UserAccount CurrentAccount
        {
            get
            {
                return ViewParameters.User;
            }
        }


        protected ITenantWebConfiguration TenantConfiguration
        {
            get
            {
                return ViewParameters.TenantConfiguration;
            }
        }

        public Task<Role> CurrentRole
        {
            get
            {
                return ViewParameters.CurrentRole;
            }
        }

        public ILogger<T> Logger
        {
            get;
        }
        public EveAuthBaseController(ILogger<T> logger,IViewParameterProvider parameters)
        {
            m_viewParameters = parameters;
            Logger = logger;
        }
    }
}