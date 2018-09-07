using System;

namespace Eve.ESI.Standard
{
    [Flags]
    public enum eESITokenRefreshResponseStatus
    {
        Success = 0x0000,
        
        Revoked = 0x0001,
        FailedToVerify = 0x0002,
        FailedToRefresh = 0x0004,
        OwnerChange = 0x0008,
        HttpError = 0x00014,
        NullResponseToken = 0x0024,
        NoScopeFound = 0x0040,
        NoTokenAvaliable = 0x0080,

    }
    public class ESITokenRefreshResponse
    {
        public string Token { get; }
        public eESITokenRefreshResponseStatus Status { get; }
        public int ExpiresIn { get; set; }
        public System.Net.HttpStatusCode Response { get; }

        public ESITokenRefreshResponse(System.Net.HttpStatusCode httpResponse)
        {
            Response = httpResponse;
            Status = eESITokenRefreshResponseStatus.HttpError;
        }

        public ESITokenRefreshResponse(eESITokenRefreshResponseStatus status)
        {
            Status = status;
        }

        public ESITokenRefreshResponse(string token,int expiresIn)
        {
            Status = eESITokenRefreshResponseStatus.Success;
            Token = token;
            ExpiresIn = expiresIn;
        }
    }
}
