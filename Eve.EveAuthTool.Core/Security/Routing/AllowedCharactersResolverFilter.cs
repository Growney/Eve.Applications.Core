
using Eve.ESI.Standard;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Eve.EveAuthTool.Core.Security.Routing
{
    public class AllowedCharactersResolverFilter : TypeFilterAttribute
    {
        public AllowedCharactersResolverFilter() : base(typeof(AllowedCharactersResolverFilterImpl))
        {

        }
        private class AllowedCharactersResolverFilterImpl : IActionFilter
        {
            private readonly IESIAuthenticatedConfig m_esiConfig;
            private readonly IStaticDataCache m_cache;
            private readonly ITenantControllerProvider m_provider;

            public AllowedCharactersResolverFilterImpl(IESIAuthenticatedConfig esiConfig,IStaticDataCache cache, ITenantControllerProvider provider)
            {
                m_esiConfig = esiConfig;
                m_cache = cache;
                m_provider = provider;
                
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
                ICommandController tenantController = m_provider.GetController(context);
                Dictionary<long, AuthenticatedEntity> characters = new Dictionary<long, AuthenticatedEntity>();
                HashSet<long> hashedAccountCharacters = new HashSet<long>();

                List<AuthenticatedEntity> avaliableCharacters = new List<AuthenticatedEntity>();

                if (context.User.Identity.IsAuthenticated)
                {
                    List<AuthenticatedEntity> accountCharacters = AuthenticatedEntity.GetForAccount(m_esiConfig, tenantController, m_cache, accountGuid: context.User.Identity.Name);
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
