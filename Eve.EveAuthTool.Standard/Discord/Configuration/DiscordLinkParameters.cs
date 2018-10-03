namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public class DiscordLinkParameters
    {
        public Gware.Standard.Web.Tenancy.Tenant Tenant { get; }

        public DiscordLinkParameters(Gware.Standard.Web.Tenancy.Tenant tenant)
        {
            Tenant = tenant;
        }
    }
}
