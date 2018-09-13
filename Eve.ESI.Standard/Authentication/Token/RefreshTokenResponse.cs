namespace Eve.ESI.Standard.Authentication.Token
{
    public class RefreshTokenResponse
    {
        public System.Net.HttpStatusCode ResponseCode { get;internal set; }
        internal AuthenticationToken Token { get; set; }
    }
}
