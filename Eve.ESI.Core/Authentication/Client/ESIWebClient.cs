using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.DataItem;
using Eve.ESI.Standard.Authentication.Token;
using Gware.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Eve.ESI.Core.Authentication.Client
{
    public class ESIWebClient : IESIAuthenticationClient
    {
        private readonly string m_esiUrl;
        private readonly string m_authUrl;
        private readonly string m_clientID;
        private readonly string m_secret;
        private readonly string m_verifyUrl;

        private readonly IHostingEnvironment m_enviroment;

        public ESIWebClient(IHostingEnvironment enviroment, string esiUrl,string authUrl, string clientID, string secret, string verifyUrl)
        {
            m_esiUrl = esiUrl;
            m_authUrl = authUrl;
            m_clientID = clientID;
            m_secret = secret;
            m_verifyUrl = verifyUrl;
            m_enviroment = enviroment;
        }
        public async Task<AuthenticationToken> RequestToken(string code)
        {
            AuthenticationToken token = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(m_authUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = CreateApplicationAuthenticationHeader(m_clientID, m_secret);
                HttpResponseMessage message = await client.PostAsync($"{m_authUrl}/token", CreateAuthContent(code));

                if (message.IsSuccessStatusCode)
                {
                    token = await message.Content.ReadAsAsync<AuthenticationToken>();
                }
            }
            return token;
        }
        public async Task<TokenCharacterInfo> VerifyToken(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(m_verifyUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = CreateVerifyAuthorisation(token);
                HttpResponseMessage message = await client.GetAsync($"{m_verifyUrl}/verify/");

                TokenCharacterInfo info = await message.Content.ReadAsAsync<TokenCharacterInfo>();
                return info;
            }
        }
        public async Task<ESITokenRefreshResponse> RefreshToken(string refreshToken)
        {
            ESITokenRefreshResponse retVal;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(m_authUrl);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = CreateApplicationAuthenticationHeader(m_clientID, m_secret);
                    HttpResponseMessage message = await client.PostAsync($"{m_authUrl}/token", CreateRefreshToken(refreshToken));

                    if (message.IsSuccessStatusCode)
                    {
                        AuthenticationToken token = await message.Content.ReadAsAsync<AuthenticationToken>();
                        if (token != null)
                        {
                            retVal = new ESITokenRefreshResponse(token.Access_Token, token.Expires_In);
                        }
                        else
                        {
                            retVal = new ESITokenRefreshResponse(eESITokenRefreshResponseStatus.NullResponseToken);
                        }
                    }
                    else
                    {
                        retVal = new ESITokenRefreshResponse(message.StatusCode);
                    }
                }
            }
            catch
            {
                retVal = new ESITokenRefreshResponse(eESITokenRefreshResponseStatus.FailedToRefresh);
            }
            return retVal;
        }

        public async Task<T> GetSingleResponseAsync<T,K>(ESICallParameters parameters) 
            where T : ESICallResponse<K>, new()
            where K : ESIItemBase
        {
            using (HttpClient client = GetHttpClient(m_esiUrl, parameters))
            {
                HttpResponseMessage message = await client.GetAsync($"{m_esiUrl}{parameters.GetParametersUrl()}");
                T retVal = ParseHTTPMessage<T>(parameters.GetGuid(), m_enviroment, message);
                if(retVal.ResponseCode == System.Net.HttpStatusCode.OK)
                {
                    retVal.SetData(await message.Content.ReadAsAsync<K>());
                }
                return retVal;
            }
        }
        public async Task<T> GetCollectionResponseAsync<T, K>(ESICallParameters parameters) 
            where T : ESICollectionCallResponse<K>, new() 
            where K : ESIItemBase
        {
            using (HttpClient client = GetHttpClient(m_esiUrl, parameters))
            {
                HttpResponseMessage message = await client.GetAsync($"{m_esiUrl}{parameters.GetParametersUrl()}");
                T retVal = ParseHTTPMessage<T>(parameters.GetGuid(), m_enviroment, message);
                if (retVal.ResponseCode == System.Net.HttpStatusCode.OK)
                {
                    retVal.SetData(await message.Content.ReadAsAsync<IEnumerable<K>>());
                }
                return retVal;
            }
        }
        public async Task<T> GetIntegerCollectionResponse<T,K>(ESICallParameters parameters)
            where T : ESIIntegerCollectionCallResponse<K>, new()
            where K : IntegerCollectionItem, new()
        {
            using (HttpClient client = GetHttpClient(m_esiUrl, parameters))
            {
                HttpResponseMessage message = await client.GetAsync($"{m_esiUrl}{parameters.GetParametersUrl()}");
                T retVal = ParseHTTPMessage<T>(parameters.GetGuid(),m_enviroment, message);
                if (retVal.ResponseCode == System.Net.HttpStatusCode.OK)
                {
                    retVal.SetData(await message.Content.ReadAsAsync<IEnumerable<int>>());
                }
                return retVal;
            }
        }

        private static T ParseHTTPMessage<T>(Guid parameterHash,IHostingEnvironment environment, System.Net.Http.HttpResponseMessage message) where T : ESICallResponse, new()
        {
            T retVal = new T
            {
                Uri = (environment.IsDevelopment()) ? message.RequestMessage.RequestUri.ToString() : message.RequestMessage.RequestUri.Host,
                Executed = DateTime.UtcNow,
                ResponseCode = message.StatusCode,
                CallID = parameterHash,
            };
            if (message.Content.Headers.TryGetValues("Last-Modified", out IEnumerable<string> lastModifiedValues))
            {
                if (DateTime.TryParse(lastModifiedValues.First(), out DateTime lastModified))
                {
                    retVal.LastModified = lastModified.ToUniversalTime();
                }
            }

            if (message.Content.Headers.TryGetValues("Expires", out IEnumerable<string> expiredValues))
            {
                if (DateTime.TryParse(expiredValues.First(), out DateTime expires))
                {
                    retVal.Expires = expires.ToUniversalTime();
                }
            }

            if (message.Headers.TryGetValues("X-Pages", out IEnumerable<string> pagesValues))
            {
                if (Int32.TryParse(pagesValues.First(), out int pages))
                {
                    retVal.Pages = pages;
                }
            }

            if (message.Headers.TryGetValues("ETag", out IEnumerable<string> etagValues))
            {
                retVal.ETag = etagValues.First();
            }

            return retVal;
        }

        private HttpClient GetHttpClient(string apiUrl,ESICallParameters parameters)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(m_esiUrl);
            parameters.SetHeaders(client);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public static string GetAuthenticationUrl(string authUrl, string callBackURL, string clientID, string scope, string state = "noState")
        {
            return $"{authUrl}/authorize?response_type=code&redirect_uri={callBackURL}&client_id={clientID}&scope={scope}&state={state}";
        }
        public static AuthenticationHeaderValue CreateVerifyAuthorisation(string token)
        {
            return new AuthenticationHeaderValue("Bearer", token);
        }
        private static StringContent CreateAuthContent(string code)
        {
            return new StringContent($"grant_type=authorization_code&code={code}", Encoding.UTF8, "application/x-www-form-urlencoded");
        }
        private static StringContent CreateRefreshToken(string code)
        {
            return new StringContent($"grant_type=refresh_token&refresh_token={code}", Encoding.UTF8, "application/x-www-form-urlencoded");
        }
        private static AuthenticationHeaderValue CreateApplicationAuthenticationHeader(string clientID, string secretKey)
        {
            string authString = $"{clientID}:{secretKey}";
            byte[] authBytes = Encoding.ASCII.GetBytes(authString);
            return new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(authBytes));
        }

    }
}
