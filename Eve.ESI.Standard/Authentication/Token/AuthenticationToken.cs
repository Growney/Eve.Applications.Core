namespace Eve.ESI.Standard.Authentication.Token
{

    public class AuthenticationToken 
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_In { get; set; }
        public string Refresh_Token { get; set; }
    }
}
