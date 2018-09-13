using System;

namespace Eve.ESI.Standard.Authentication.Token
{
    internal class CorporationTokenEntity : ITokenEntity
    {
        public byte EntityType => (byte)eESIEntityType.corporation;
        public long EntityID { get; private set; }
        public string EntityName { get; private set; }
        public DateTime ExpiresOn { get; private set; }
        public string Scopes { get; private set; }
        public string TokenType { get; private set; }
        public string CharacterOwnerHash { get; private set; }

        public CorporationTokenEntity(long corporationID,string corporationName,TokenCharacterInfo charInfo)
        {
            EntityID = corporationID;
            EntityName = corporationName;
            ExpiresOn = charInfo.ExpiresOn;
            Scopes = charInfo.Scopes;
            TokenType = charInfo.TokenType;
            CharacterOwnerHash = charInfo.CharacterOwnerHash;
        }
    }
}
