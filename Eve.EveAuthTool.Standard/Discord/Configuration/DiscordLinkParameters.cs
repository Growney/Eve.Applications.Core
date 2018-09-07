using Gware.Standard.Web.Tenancy;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public class DiscordLinkParameters
    {
        public Tenant Tenant { get; }

        public DiscordLinkParameters(Tenant tenant)
        {
            Tenant = tenant;
        }
    }
}
