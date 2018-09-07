using Gware.Standard.Web.OAuth;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public class DiscordOAuthRequestArguments : OAuthRequestArguments
    {
        public override string GetAuthenticationUrl(IOAuthConfiguration config, string state)
        {
            return $"{config.AuthUrl}/authorize?response_type=code&client_id={config.ClientID}&redirect_uri={config.CallBackUrl}&state={state}&scope=identify guilds guilds.join";
        }
    }
}
