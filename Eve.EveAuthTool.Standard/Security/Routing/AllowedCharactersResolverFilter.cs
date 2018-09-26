﻿
using Eve.ESI.Standard;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Security.Routing
{
    public class AllowedCharactersResolverFilter : TypeFilterAttribute
    {
        public AllowedCharactersResolverFilter() : base(typeof(AllowedCharactersResolverFilterImpl))
        {

        }
        private class AllowedCharactersResolverFilterImpl : IActionFilter
        {
            private readonly IControllerParameters m_parameters;

            public AllowedCharactersResolverFilterImpl(IControllerParameters controllerParameters)
            {
                m_parameters = controllerParameters;


            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                CalculateCharacters(context.HttpContext);
            }

            private void CalculateCharacters(HttpContext context)
            {
                ViewParameterPackage viewParameters = m_parameters.CreateViewParameters(context);
                Dictionary<long, AuthenticatedEntity> characters = new Dictionary<long, AuthenticatedEntity>();
                HashSet<long> hashedAccountCharacters = new HashSet<long>();

                List<AuthenticatedEntity> avaliableCharacters = new List<AuthenticatedEntity>();

                if (context.User.Identity.IsAuthenticated)
                {
                    List<AuthenticatedEntity> accountCharacters = AuthenticatedEntity.GetForAccount(viewParameters.ESIConfiguration,viewParameters.TenantController,viewParameters.Cache,viewParameters.PublicDataProvider,accountGuid: context.User.Identity.Name);
                    avaliableCharacters.AddRange(accountCharacters);
                    for (int i = 0; i < accountCharacters.Count; i++)
                    {
                        if (!hashedAccountCharacters.Contains(accountCharacters[i].EntityID))
                        {
                            hashedAccountCharacters.Add(accountCharacters[i].EntityID);
                        }
                    }
                }

                for (int i = 0; i < avaliableCharacters.Count; i++)
                {
                    AuthenticatedEntity character = avaliableCharacters[i];
                    if (!characters.ContainsKey(character.EntityID))
                    {
                        characters.Add(character.EntityID, character);
                    }
                }

                context.Features.Set<AllowedCharacters>(new AllowedCharacters(characters, hashedAccountCharacters));
            }
        }
    }
}