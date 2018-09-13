using System;

namespace Eve.ESI.Standard.Authentication.Token
{
    internal interface ITokenEntity
    {
        byte EntityType { get; }
        long EntityID { get; }
        string EntityName { get; }
        DateTime ExpiresOn { get; }
        string Scopes { get; }
        string TokenType { get;  }
        string CharacterOwnerHash { get; }
    }
}
