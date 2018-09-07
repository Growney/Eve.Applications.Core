using Eve.ESI.Standard.AuthenticatedData;
using System.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Security
{
    public class AllowedCharacters
    {
        public Dictionary<long, AuthenticatedEntity> Characters { get; }
        public HashSet<long> AccountCharacters { get; }

        public AllowedCharacters(Dictionary<long, AuthenticatedEntity> characters, HashSet<long> accountCharacters)
        {
            Characters = characters;
            AccountCharacters = accountCharacters;
        }

        public IEnumerable<AuthenticatedEntity> GetCharacters()
        {
            return Characters.Values;
        }

        public IEnumerable<AuthenticatedEntity> GetAccountCharacters()
        {
            List<AuthenticatedEntity> retVal = new List<AuthenticatedEntity>();

            foreach (long characterID in AccountCharacters)
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
