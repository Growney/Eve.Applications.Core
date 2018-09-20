﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Eve.ESI.Core.Authentication.Request;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.Authentication;
using Eve.ESI.Standard.Authentication.Account;
using Eve.ESI.Standard.DataItem;
using Eve.ESI.Standard.DataItem.Alliance;
using Eve.ESI.Standard.DataItem.Character;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.Authentication.Token;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.GUI.Web.Controllers.Helpers;
using Eve.EveAuthTool.GUI.Web.Models.Registration;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Web.OAuth;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Eve.EveAuthTool.Standard.Security;

namespace Eve.EveAuthTool.GUI.Web.Controllers
{ 
    public class RegistrationController : EveAuthBaseController
    {
        public IArgumentsStore<ESITokenRequestParameters> TokenStore { get; }
        public IArgumentsStore<OAuthRequestArguments> OAuthArgStore { get; }
        public IScopeGroupProvider Scopes { get; }
        public RegistrationController(IArgumentsStore<ESITokenRequestParameters> tokenStore,IArgumentsStore<OAuthRequestArguments> oAuthArgStore,IScopeGroupProvider scopes,IViewParameterProvider parameters)
            :base(parameters)
        {
            TokenStore = tokenStore;
            OAuthArgStore = oAuthArgStore;
            Scopes = scopes;
        }
        public IActionResult Group()
        {
            return View();
        }
        public async Task<IActionResult> CharacterLogin(string state)
        {
            ESIAuthRequestArguments arguments = OAuthArgStore.ReCallArguments(state) as ESIAuthRequestArguments;

            if (arguments != null && !String.IsNullOrWhiteSpace(arguments.Code))
            {
                OAuthArgStore.DiscardArguments(state);
                ESIToken token = await ESIToken.Authenticate(ESIConfiguration, TenantController, eESIEntityType.character, arguments.Code);
                bool success = await SignInToken(token);
                if (success)
                {
                    return RedirectToAction(Startup.c_defaultAction, Startup.c_defaultController);
                }
                else
                {
                    return new JsonResult("Character account not found");
                }

            }
            return new JsonResult("Bad ESI Authorisation");
        }
        private async Task SignInAccount(UserAccount account)
        {
            List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.AccountGuid.ToString())
                    };

            claims.AddRange(await GetRelevantClaims(account));

            ClaimsIdentity identity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationProperties props = new AuthenticationProperties()
            {
                IsPersistent = true,
                AllowRefresh = true

            };
            await HttpContext.SignInAsync(principal, props);
        }
        private async Task<List<Claim>> GetRelevantClaims(UserAccount account)
        {
            List<Claim> retVal = new List<Claim>();
            if (IsTenant)
            {
                eRulePermission activePermissions = eRulePermission.None;
                List<AuthenticatedEntity> characters = AuthenticatedEntity.GetForAccount(ESIConfiguration, TenantController, Cache, PublicDataProvider, account);
                foreach(AuthenticatedEntity character in characters)
                {
                    Role role = await AuthRule.GetEntityRole(ESIConfiguration,TenantController,Cache,PublicDataProvider, character);
                    if (role != null)
                    {
                        activePermissions |= role.Permissions;
                        
                    }
                }
                foreach (eRulePermission permission in Enum.GetValues(typeof(eRulePermission)))
                {
                    if (activePermissions.HasFlag(permission))
                    {
                        retVal.Add(new Claim(ClaimTypes.Role, permission.ToString()));
                    }
                }
                
            }
            return retVal;
        }
        private async Task<bool> SignInToken(ESIToken token)
        {
            bool retVal = true;
            
            if (token != null)
            {
                UserAccount loginAccount = UserAccount.ForCharacterID(TenantController, token.EntityID);
                if (loginAccount != null)
                {
                    await SignInAccount(loginAccount);
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(Startup.c_defaultAction, Startup.c_defaultController);
        }

        public IActionResult Denied()
        {
            return View();
        }
        public IActionResult EveLogin()
        {
            ESIAuthRequestArguments args = HttpContext.GetLocalArguments<ESIAuthRequestArguments>(
                redirectPath: "Registration/CharacterLogin");

            return Redirect(args.GetAuthenticationUrl(
                state: OAuthArgStore.StoreArguments(args),
                config: ESIConfiguration));
        }
        public async Task<IActionResult> CharacterRegister(string state)
        {
            ESIAuthRequestArguments arguments = OAuthArgStore.ReCallArguments(state) as ESIAuthRequestArguments;

            if (arguments != null && !String.IsNullOrWhiteSpace(arguments.Code))
            {
                OAuthArgStore.DiscardArguments(state);

                ESIToken token = await ESIToken.Authenticate(ESIConfiguration, TenantController, eESIEntityType.character, arguments.Code);

                if (token != null)
                {
                    bool allowedByTenant = true;
                    if (IsTenant)
                    {
                        allowedByTenant = false;
                        AuthenticatedEntity entity = AuthenticatedEntity.FromToken(ESIConfiguration, TenantController, Cache,PublicDataProvider, token);
                        Role role = await AuthRule.GetEntityRole(ESIConfiguration,TenantController,Cache,PublicDataProvider, entity);
                        if (role != null)
                        {
                            if (role.Permissions.HasFlag(eRulePermission.Register))
                            {
                                allowedByTenant = true;
                            }
                        }
                    }
                    if (allowedByTenant)
                    {
                        UserAccount loginAccount = UserAccount.ForCharacterID(TenantController, token.EntityID);

                        if (HttpContext.User.Identity.IsAuthenticated)
                        {
                            UserAccount currentAccount = UserAccount.ForGuid(TenantController, HttpContext.User.Identity.Name);
                            if (currentAccount != null)
                            {
                                if (loginAccount != null)
                                {
                                    if (currentAccount.Id != loginAccount.Id)
                                    {
                                        await HttpContext.SignOutAsync();
                                    }
                                }
                                else
                                {
                                    currentAccount.AddToken(TenantController, token);
                                    loginAccount = currentAccount;
                                }
                            }
                        }

                        if (loginAccount == null)
                        {

                            loginAccount = UserAccount.CreateNew();
                            loginAccount.AddToken(TenantController, token);


                        }

                        await SignInAccount(loginAccount);
                        return RedirectToAction("Welcome", "Home");
                    }
                    else
                    {
                        return new JsonResult($"Character access denied by {CurrentTenant.DisplayName}");
                    }
                }
                else
                {
                    return new JsonResult("Bad ESI Authorisation");
                }
            }
            else
            {
                return new JsonResult("Bad ESI Authorisation");
            }
        }
        public IActionResult Character(uint[] selectedScopes)
        {
            ESIAuthRequestArguments args = HttpContext.GetLocalArguments<ESIAuthRequestArguments>(
                redirectPath: "Registration/CharacterRegister");
            args.Scopes = Scopes.GetCharacterScopes();

            return Redirect(args.GetAuthenticationUrl(
                state: OAuthArgStore.StoreArguments(args),
                config: ESIConfiguration));
        }
        public IActionResult Alliance(GroupRegistrationOptions options)
        {
            ESIAuthRequestArguments args = HttpContext.GetLocalArguments<ESIAuthRequestArguments>(
                redirectPath: $"Registration/AllianceProposal");
            args.Scopes = ESIScopeHelper.Merge(Scopes.GetAllianceScopes(options.SelectedScopes), (options.RegisterCharacter) ? Scopes.GetCharacterScopes() : null);

            List<eESIEntityType> createFrom = new List<eESIEntityType>() { eESIEntityType.alliance };
            if (options.RegisterCharacter)
            {
                createFrom.Add(eESIEntityType.character);
            }
            args.CreateFromResponse = createFrom;

            return Redirect(args.GetAuthenticationUrl(
                state: OAuthArgStore.StoreArguments(args),
                config: ESIConfiguration));
        }
        public IActionResult Corporation(GroupRegistrationOptions options)
        {
            ESIAuthRequestArguments args = HttpContext.GetLocalArguments<ESIAuthRequestArguments>(
                redirectPath: $"Registration/CorporationProposal");
            args.Scopes = ESIScopeHelper.Merge(Scopes.GetCorporationScopes(options.SelectedScopes), (options.RegisterCharacter) ? Scopes.GetCharacterScopes() : null);

            List<eESIEntityType> createFrom = new List<eESIEntityType>() { eESIEntityType.corporation };
            if (options.RegisterCharacter)
            {
                createFrom.Add(eESIEntityType.character);
            }
            args.CreateFromResponse = createFrom;

            return Redirect(args.GetAuthenticationUrl(
                state: OAuthArgStore.StoreArguments(args),
                config: ESIConfiguration));
        }
        public IActionResult TestPropose()
        {
            TenantProposal proposal = new TenantProposal()
            {
                EntityType = eESIEntityType.corporation,
                EntityID = 123451243,
                TokenGuid = string.Empty,
                Name = "thisisatestname",
                DisplayName = "Test display name"
            };
            return View("Propose", proposal);
        }
        public async Task<IActionResult> AllianceProposal(string state)
        {
            ESIAuthRequestArguments arguments = OAuthArgStore.ReCallArguments(state) as ESIAuthRequestArguments;

            if (arguments != null)
            {
                OAuthArgStore.DiscardArguments(state);
                ESIToken token = await ESIToken.Authenticate(ESIConfiguration, TenantController, eESIEntityType.alliance, arguments.Code);
                if (token != null)
                {
                    if (token.AllianceID != 0)
                    {
                        //We don't pass a controller in here as we don't want to put any cache data into the current tenant that might be sensitive
                        ESICallResponse<CharacterRoles> roles = await CharacterRoles.GetCharacterRoles(ESIConfiguration.Client, null, token.CharacterID, () => { return token.GetAuthenticationToken(ESIConfiguration, TenantController); });
                        if (roles.IsSuccess)
                        {
                            if (roles.Data.HasRole(eESIRole.Director))
                            {
                                if (!TenantConfiguration.DoesTenantExist((int)token.EntityType, token.EntityID))
                                {
                                    ESICallResponse<AllianceInfo> allianceInfo = await PublicDataProvider.GetAllianceInfo(token.AllianceID);
                                    if (allianceInfo.HasData)
                                    {
                                        if (allianceInfo.Data.ExecutorCorporationId == token.CorporationID)
                                        {
                                            ESITokenRequestParameters parameters = new ESITokenRequestParameters()
                                            {
                                                Token = token,
                                                CreateFrom = arguments.CreateFromResponse
                                            };
                                            string validName = Tenant.MakeValidTenantName(allianceInfo.Data.Name);
                                            TenantProposal proposal = new TenantProposal()
                                            {
                                                EntityID = token.AllianceID,
                                                EntityType = eESIEntityType.alliance,
                                                Name = validName,
                                                DisplayName = allianceInfo.Data.Name,
                                                TokenGuid = TokenStore.StoreArguments(parameters)
                                            };
                                            return View("Propose", proposal);
                                        }
                                        else
                                        {
                                            return new JsonResult("Must be a director of the executor corp to register as an alliance");
                                        }
                                    }
                                    else
                                    {
                                        return new JsonResult("Error getting Alliance Info");
                                    }
                                }
                                else
                                {
                                    return new JsonResult("Entity already has a tenant");
                                }
                            }
                            else
                            {
                                return new JsonResult("Must have in game role director to register");
                            }

                        }
                        else
                        {
                            return new JsonResult("Could not get character roles");
                        }
                    }
                    else
                    {
                        return new JsonResult("Corporation not in an alliance");
                    }

                }
                else
                {
                    return new JsonResult("ESI Authentication error");
                }

            }
            else
            {
                return  new JsonResult("Bad ESI Authorisation");
            }

        }
        public async Task<IActionResult> CorporationProposal(string state)
        {
            ESIAuthRequestArguments arguments = OAuthArgStore.ReCallArguments(state) as ESIAuthRequestArguments;

            if (arguments != null)
            {
                OAuthArgStore.DiscardArguments(state);
                ESIToken token = await ESIToken.Authenticate(ESIConfiguration, TenantController, eESIEntityType.corporation, arguments.Code);
                if (token != null)
                {
                    if (!TenantConfiguration.DoesTenantExist((int)token.EntityType, token.EntityID))
                    {
                        ESICallResponse<CharacterRoles> roles = await CharacterRoles.GetCharacterRoles(ESIConfiguration.Client, null, token.CharacterID, () => { return token.GetAuthenticationToken(ESIConfiguration, TenantController); });
                        if (roles.IsSuccess)
                        {
                            if (roles.Data.HasRole(eESIRole.Director))
                            {
                                ESICallResponse<CorporationInfo> corporationInfo = await PublicDataProvider.GetCorporationInfo(token.CorporationID);
                                if (corporationInfo.HasData)
                                {
                                    ESITokenRequestParameters parameters = new ESITokenRequestParameters()
                                    {
                                        Token = token,
                                        CreateFrom = arguments.CreateFromResponse
                                    };
                                    string validName = Tenant.MakeValidTenantName(corporationInfo.Data.Name);
                                    TenantProposal proposal = new TenantProposal()
                                    {
                                        EntityType = eESIEntityType.corporation,
                                        Name = validName,
                                        DisplayName = corporationInfo.Data.Name,
                                        EntityID = token.EntityID,
                                        TokenGuid = TokenStore.StoreArguments(parameters)
                                    };
                                    return View("Propose", proposal);
                                }
                                else
                                {
                                    return new JsonResult("Error getting corporation info");
                                }
                            }
                            else
                            {
                                return new JsonResult("Must have in game role director to register");
                            }

                        }
                        else
                        {
                            return new JsonResult("Could not get character roles");
                        }


                    }
                    else
                    {
                        return new JsonResult("Entity already has a tenant");
                    }
                }
                else
                {
                    return new JsonResult("ESI Authentication error");
                }

            }
            else
            {
                return new JsonResult("Bad ESI Authorisation");
            }
        }
        public IActionResult ValidateTenant(string name, string displayname)
        {
            return new JsonResult(ValidateTenantName(name, displayname));
        }
        private int ValidateTenantName(string name, string displayName)
        {
            int retVal = 0;
            if (String.IsNullOrWhiteSpace(name))
            {
                retVal = 1;
            }
            else if (String.IsNullOrWhiteSpace(displayName))
            {
                retVal = 2;
            }
            else if (name.Length < 4 || name.Length > 100)
            {
                retVal = 3;
            }
            else if (displayName.Length > 100)
            {
                retVal = 4;
            }
            else if (!Tenant.IsValidTenantName(name))
            {
                retVal = 5;
            }
            else if (TenantConfiguration.DoesTenantExist(name))
            {
                retVal = 6;
            }
            return retVal;
        }
        private int ValidateTenant(string name, string displayName, int entityType, long entityID)
        {
            int retVal = ValidateTenantName(name, displayName);
            if (retVal == 0)
            {
                if (TenantConfiguration.DoesTenantExist(entityType, entityID))
                {
                    retVal = 7;
                }
            }
            return retVal;
        }
        [HttpPost]
        public async Task<IActionResult> AcceptTenant(TenantProposal proposal)
        {
            TenantCreationResult result = new TenantCreationResult();
            ESITokenRequestParameters parameters = TokenStore.ReCallArguments(proposal.TokenGuid);
            ESIToken token = parameters?.Token;
            if (token != null)
            {
                int validation = ValidateTenant(proposal.Name, proposal.DisplayName, (int)token.EntityType, token.EntityID);
                if (validation == 0)
                {
                    bool createdTenant = await TenantConfiguration.CreateTenant(proposal.Name, proposal.DisplayName, (int)token.EntityType, token.EntityID);
                    if (createdTenant)
                    {
                        result.IsSuccess = true;
                        result.RedirectUrl = TenantConfiguration.GetTenantRedirect(proposal.Name, $"Registration/OnCreate?tokenGuid={proposal.TokenGuid}");
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Validation = -1;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Validation = validation;
                }

            }
            else
            {
                result.IsSuccess = false;
                result.Validation = -2;
            }
            
            

            return new JsonResult(result);
        }
        public async Task<IActionResult> OnCreate(string tokenGuid)
        {
            ESITokenRequestParameters parameters = TokenStore.ReCallArguments(tokenGuid);
            ESIToken token = parameters?.Token;
            if (token != null)
            {
                TokenStore.DiscardArguments(tokenGuid);
                token.Save(TenantController);

                AuthRule.CreateAdminRule(TenantController, token.CorporationID);
                AuthRule.CreateMemberRule(TenantController, token.EntityID, token.EntityType);
                AuthRule.CreateStandingRule(TenantController, token.EntityID, token.EntityType);

                if (parameters.CreateFrom != null)
                {
                    foreach(eESIEntityType entityType in parameters.CreateFrom)
                    {
                        if(entityType != token.EntityType)
                        {
                            ESIToken tokenCopy = token.Copy(entityType);
                            tokenCopy.Save(TenantController);

                            if(entityType == eESIEntityType.character)
                            {
                                UserAccount newAccount = UserAccount.CreateNew();
                                newAccount.AddToken(TenantController, tokenCopy);
                                await SignInAccount(newAccount);
                            }
                        }
                    }
                }
                return RedirectToAction("Welcome", "Home");
                
            }
            else
            {
                return new JsonResult("Bad ESI Authorisation");
            }
        }
        public IActionResult OAuthCallBack(string code, string state)
        {
            OAuthRequestArguments arguments = OAuthArgStore.ReCallArguments(state) as OAuthRequestArguments;
            if (arguments != null)
            {
                arguments.Code = code;
                return Redirect(arguments.GetRedirectString(state));
            }
            else
            {
                return new JsonResult("Bad OAuth");
            }
        }
    }
}