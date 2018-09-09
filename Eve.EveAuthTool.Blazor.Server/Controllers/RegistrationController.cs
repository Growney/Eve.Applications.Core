using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Eve.ESI.Core.Authentication.Request;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Token;
using Eve.EveAuthTool.Blazor.Client.Services;
using Eve.EveAuthTool.Core.Security.Routing;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Web.OAuth;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Eve.EveAuthTool.Blazor.Server.Controllers
{
    public class RegistrationController : BaseController
    {
        public IArgumentsStore<OAuthRequestArguments> ArgumentsStore { get; }
        public RegistrationController(ControllerParameters parameters,
            IArgumentsStore<OAuthRequestArguments> argumentsStore)
            : base(parameters)
        {
            ArgumentsStore = argumentsStore;
        }
        public string GetLoginRedirect()
        {
            ESIAuthRequestArguments args = HttpContext.GetLocalArguments<ESIAuthRequestArguments>(
                redirectPath: "api/Registration/Login");

            return args.GetAuthenticationUrl(
                state: ArgumentsStore.StoreArguments(args),
                config: ESIConfiguration);
        }


        public SessionState GetSessionState()
        {
            return new SessionState(null)
            {
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                HasTenant = CurrentTenant != null,
                TenantName = CurrentTenant?.DisplayName,
                TenantEntityID = CurrentTenant?.EntityId,
                TenantEntityType = CurrentTenant?.EntityType
            };
        }
        public IActionResult OAuthCallBack(string code, string state)
        {
            OAuthRequestArguments arguments = ArgumentsStore.ReCallArguments(state) as OAuthRequestArguments;
            if (arguments != null)
            {
                arguments.Code = code;
                return Redirect(arguments.GetRedirectString(state));
            }
            else
            {
                return RedirectToError("Bad OAuth Request");
            }
        }

        public async Task<IActionResult> Login(string state)
        {
            ESIAuthRequestArguments arguments = ArgumentsStore.ReCallArguments(state) as ESIAuthRequestArguments;

            if (arguments != null && !String.IsNullOrWhiteSpace(arguments.Code))
            {
                ArgumentsStore.DiscardArguments(state);
                ESIToken token = await ESIToken.Authenticate(ESIConfiguration, TenantController, eESIEntityType.character, arguments.Code);

                if (token != null)
                {
                    UserAccount loginAccount = UserAccount.ForCharacterID(TenantController, token.EntityID);
                    if (loginAccount != null)
                    {
                        await SignInAccount(loginAccount);
                        return LocalRedirect("/");
                    }
                    else
                    {
                        return RedirectToError("Character account not found");
                    }
                }

            }
            return RedirectToError("Bad ESI Authorisation");

        }
        private async Task SignInAccount(UserAccount account)
        {
            List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.AccountGuid.ToString())
                    };

            claims.AddRange(GetRelevantClaims(account));

            ClaimsIdentity identity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationProperties props = new AuthenticationProperties()
            {
                IsPersistent = true,
                AllowRefresh = true

            };
            await HttpContext.SignInAsync(principal, props);
        }
        private List<Claim> GetRelevantClaims(UserAccount account)
        {
            List<Claim> retVal = new List<Claim>();
            if (IsTenant)
            {
                
            }
            return retVal;
        }
    }
}