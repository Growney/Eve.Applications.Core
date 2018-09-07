using Eve.ESI.Standard.AuthenticatedData;
using System.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Security
{
    public interface IAllowedCharactersProvider
    {
        IEnumerable<AuthenticatedEntity> GetCharacters();
        IEnumerable<AuthenticatedEntity> GetAccountCharacters();
        bool AllowedCharacter(long characterID);
        AuthenticatedEntity GetCharacter(long characterID);
    }
}
