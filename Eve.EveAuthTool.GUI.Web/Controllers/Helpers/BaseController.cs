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
using Eve.EveAuthTool.Standard.Helpers;

namespace Eve.EveAuthTool.GUI.Web.Controllers.Helpers
{
    public class EveAuthBaseController<T> : Controller
    {
        protected ICommandController TenantController
        {
            get
            {
                return m_scopes.TenantController;
            }
        }
        protected IPublicDataProvider PublicDataProvider
        {
            get
            {
                return m_singles.PublicDataProvider;
            }
        }
        protected IAllowedCharactersProvider Characters
        {
            get
            {
                return m_scopes.Characters;
            }
        }
        protected IESIAuthenticatedConfig ESIConfiguration
        {
            get
            {
                return m_singles.ESIConfiguration;
            }
        }
        protected IStaticDataCache Cache
        {
            get
            {
                return m_singles.Cache;
            }
        }

        protected bool IsTenant
        {
            get
            {
                return m_scopes.IsTenant;
            }
        }

        protected Tenant CurrentTenant
        {
            get
            {
                return m_scopes.CurrentTenant;
            }
        }
        protected UserAccount CurrentAccount
        {
            get
            {
                return m_scopes.User;
            }
        }


        protected ITenantWebConfiguration TenantConfiguration
        {
            get
            {
                return m_singles.TenantConfiguration;
            }
        }

        public Task<Role> CurrentRole
        {
            get
            {
                return m_scopes.CurrentRole;
            }
        }
        public IServiceProvider ServiceProvider
        {
            get
            {
                return m_singles.ServiceProvider;
            }
        }

        public long? MainCharacterID
        {
            get
            {
                return m_scopes.MainCharacterID;
            }
        }
        public ILogger<T> Logger { get; }
        private readonly ISingleParameters m_singles;
        private readonly IScopeParameters m_scopes;

        

        public EveAuthBaseController(ILogger<T> logger, ISingleParameters singles, IScopeParameters scopes)
        {
            m_singles = singles;
            m_scopes = scopes;
            Logger = logger;
        }
        
    }
}