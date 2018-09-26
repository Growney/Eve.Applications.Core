using Eve.ESI.Standard.AuthenticatedData;
using Eve.EveAuthTool.Standard.Security;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Security.Middleware
{
    public class AllowedCharacterProvider : IAllowedCharactersProvider
    {
        private readonly IHttpContextAccessor m_context;

        private Dictionary<long, AuthenticatedEntity> Characters
        {
            get
            {
                return m_context.HttpContext.Features.Get<AllowedCharacters>()?.Characters ?? new Dictionary<long, AuthenticatedEntity>();
            }
        }
        private HashSet<long> AccountCharacters
        {
            get
            {
                return m_context.HttpContext.Features.Get<AllowedCharacters>()?.AccountCharacters ?? new HashSet<long>();
            }
        }

        public AllowedCharacterProvider(IHttpContextAccessor context)
        {
            m_context = context;
        }

        
        
        public IEnumerable<AuthenticatedEntity> GetCharacters()
        {
            return Characters.Values;
        }

        public IEnumerable<AuthenticatedEntity> GetAccountCharacters()
        {
            List<AuthenticatedEntity> retVal = new List<AuthenticatedEntity>();

            foreach(long characterID in AccountCharacters)
            {
                if (Characters.ContainsKey(characterID))
                {
                    retVal.Add(Characters[characterID]);
                }
            }

            return retVal;
        }

        public bool AllowedCharacter(long characterID)
        {
            return Characters.ContainsKey(characterID);
        }

        public AuthenticatedEntity GetCharacter(long characterID)
        {
            return Characters[characterID];
        }
    }
}
