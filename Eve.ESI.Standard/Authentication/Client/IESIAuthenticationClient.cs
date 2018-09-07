using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.DataItem;
using Eve.ESI.Standard.Token;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.Authentication.Client
{
    public interface IESIAuthenticationClient
    {
        Task<AuthenticationToken> RequestToken(string code);
        Task<TokenCharacterInfo> VerifyToken(string token);
        Task<ESITokenRefreshResponse> RefreshToken(string refreshToken); Task<T> GetSingleResponseAsync<T, K>(Guid callID, ESICallParameters parameters)
             where T : ESICallResponse<K>, new()
             where K : ESIItemBase;
        Task<T> GetCollectionResponseAsync<T, K>(Guid callID, ESICallParameters parameters)
            where T : ESICollectionCallResponse<K>, new()
            where K : ESIItemBase;
        Task<T> GetIntegerCollectionResponse<T, K>(Guid callID, ESICallParameters parameters)
            where T : ESIIntegerCollectionCallResponse<K>, new()
            where K : IntegerCollectionItem, new();

    }
}
