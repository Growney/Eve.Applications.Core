using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Authentication
{
    public class AuthenticationToken
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_In { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }


        public static async Task<AuthenticationToken> RequestToken(IDiscordBotConfiguration configuration,string code)
        {
            AuthenticationToken token = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration.AuthUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = CreateApplicationAuthenticationHeader(configuration.ClientID, configuration.Secret);
                HttpResponseMessage message = await client.PostAsync($"{configuration.AuthUrl}/token", CreateAuthContent(configuration,code));

                if (message.IsSuccessStatusCode)
                {
                    token = await message.Content.ReadAsAsync<AuthenticationToken>();
                }
            }
            return token;
        }
        private static AuthenticationHeaderValue CreateApplicationAuthenticationHeader(string clientID, string secretKey)
        {
            string authString = $"{clientID}:{secretKey}";
            byte[] authBytes = Encoding.ASCII.GetBytes(authString);
            return new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(authBytes));
        }
        private static StringContent CreateAuthContent(IDiscordBotConfiguration configuration,string code)
        {
            return new StringContent($"redirect_uri={configuration.CallBackUrl}&grant_type=authorization_code&code={code}", Encoding.UTF8, "application/x-www-form-urlencoded");
        }
    }
}
