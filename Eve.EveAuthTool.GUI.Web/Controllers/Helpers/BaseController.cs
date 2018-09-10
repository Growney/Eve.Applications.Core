using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace Eve.EveAuthTool.GUI.Web.Controllers.Helpers
{
    public class BaseController : Controller
    {
        private IViewParameterProvider m_viewParameters;

        public ViewParameterPackage ViewParameters
        {
            get
            {
                return m_viewParameters.Package;
            }
        }
        private ICommandController m_tenantController;
        protected ICommandController TenantController
        {
            get
            {
                if (m_tenantController == null)
                {
                    m_tenantController = ViewParameters.TenantController;
                }
                return m_tenantController;
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

        public BaseController(IViewParameterProvider parameters)
        {
            m_viewParameters = parameters;
        }
    }
}