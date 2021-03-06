﻿using Eve.ESI.Standard.Account;
using Eve.EveAuthTool.Standard.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Configuration;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Helpers
{
    public interface IScopeParameters
    {
        ITypeSafeConfigurationProvider<eUserSetting> UserConfiguration { get; }
        bool IsTenant { get; }
        Tenant CurrentTenant { get; }
        ICommandController TenantController { get; }
        IAllowedCharactersProvider Characters { get; }
        bool IsAuthenticated { get; }
        long? MainCharacterID { get; }
        UserAccount User { get; }
        Task<Role> CurrentRole { get; }
        Task<DiscordRoleConfiguration> CurrentDiscordConfiguration { get; }


    }
}
