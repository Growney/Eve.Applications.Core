using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Discord
{
    public class DiscordConfiguration
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<SelectedDiscordRole> Roles { get; set; }

        public static List<DiscordConfiguration> CreateFrom(IEnumerable<DiscordRoleConfiguration> configurations, IEnumerable<DiscordRole> allRoles)
        {
            List<DiscordConfiguration> retVal = new List<DiscordConfiguration>();

            foreach(DiscordRoleConfiguration config in configurations)
            {
                retVal.Add(CreateFrom(config,allRoles));
            }

            return retVal;
        }

        public static DiscordConfiguration CreateFrom(DiscordRoleConfiguration config, IEnumerable<DiscordRole> allRoles)
        {
            return new DiscordConfiguration()
            {
                Id = config.Id,
                Name = config.Name,
                Roles = SelectedDiscordRole.CreateFrom(allRoles, config.AssignedRoles)
            };
        }

        public DiscordRoleConfiguration Create()
        {
            DiscordRoleConfiguration config = new DiscordRoleConfiguration()
            {
                Id = Id,
                Name = Name
            };
            foreach (SelectedDiscordRole role in Roles)
            {
                if (role.Selected)
                {

                    config.AssignedRoles.Add(role.ID);
                }
            }
            return config;
        }
    }
}
