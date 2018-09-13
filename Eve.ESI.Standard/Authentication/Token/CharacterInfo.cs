using System;

namespace Eve.ESI.Standard.Authentication.Token
{
    public class TokenCharacterInfo 
    {
        public long CharacterID { get; set; }
        public string CharacterName { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Scopes { get; set; }
        public string TokenType { get; set; }
        public string CharacterOwnerHash { get; set; }
    }
}
