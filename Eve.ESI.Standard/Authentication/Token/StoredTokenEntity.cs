using System;

namespace Eve.ESI.Standard.Token
{
    internal class StoredTokenEntity : ITokenEntity
    {
        public byte EntityType { get; set; }
        public long EntityID { get; set; }
        public string EntityName { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Scopes { get; set; }
        public string TokenType { get; set; }
        public string CharacterOwnerHash { get; set; }
    }
}
