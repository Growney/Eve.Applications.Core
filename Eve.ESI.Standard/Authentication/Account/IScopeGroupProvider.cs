using Eve.ESI.Standard.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.ESI.Standard.Authentication.Account
{
    public interface IScopeGroupProvider
    {
        ScopeGroup[] GetCharacterScopeGroups();
        ScopeGroup[] GetCorporationScopesGroups();
        ScopeGroup[] GetAllianceScopesGroups();

        eESIScope[] GetCharacterScopes(uint[] selected);
        eESIScope[] GetCorporationScopes(uint[] selected);
        eESIScope[] GetAllianceScopes(uint[] selected);

        eESIScope[] GetCharacterScopes();
        eESIScope[] GetCorporationScopes();
        eESIScope[] GetAllianceScopes();
    }
}
