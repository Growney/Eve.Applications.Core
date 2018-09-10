using Eve.ESI.Standard.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.ESI.Standard.Authentication.Account
{
    public interface IScopeGroupProvider
    {
        ScopeGroup[] GetCharacterScopes();
        ScopeGroup[] GetCorporationScopes();
        ScopeGroup[] GetAllianceScopes();
    }
}
