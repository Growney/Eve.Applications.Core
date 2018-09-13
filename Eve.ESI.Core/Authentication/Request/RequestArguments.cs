using Eve.ESI.Core.Authentication.Client;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication;
using Gware.Standard.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.ESI.Core.Authentication.Request
{
    public class ESIAuthRequestArguments : OAuthRequestArguments
    {
        public eESIScope[] Scopes { get; set; }
        public List<eESIEntityType> CreateFromResponse { get; set; }

        public ESIAuthRequestArguments()
        {
            Scopes = new eESIScope[0];
        }

        public override string GetAuthenticationUrl(IOAuthConfiguration config, string state)
        {
            return ESIWebClient.GetAuthenticationUrl(config.AuthUrl, config.CallBackUrl, config.ClientID, ESIScopeHelper.GetScopeString(Scopes), state);
        }
    }
}
