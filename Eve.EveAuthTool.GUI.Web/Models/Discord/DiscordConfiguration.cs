using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gware.Standard.Collections.Generic;

namespace Eve.EveAuthTool.GUI.Web.Models.Discord
{
    public class DiscordConfiguration
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string GuildInviteID { get; set; }
        public string InviteUrl { get; set; }
        public List<SelectedDiscordRole> Roles { get; set; }

        public static List<T> CreateFrom<T>(IEnumerable<DiscordRoleConfiguration> configurations, IEnumerable<DiscordRole> allRoles, IEnumerable<DiscordInvite> allinvites) where T : DiscordConfiguration,new()
        {
            return CreateFrom<T>(configurations, allRoles, allinvites.Index((x) => { return x.ID; }));
        }
        public static List<T> CreateFrom<T>(IEnumerable<DiscordRoleConfiguration> configurations, IEnumerable<DiscordRole> allRoles, Dictionary<string, DiscordInvite> allinvites) where T : DiscordConfiguration, new()
        {
            List<T> retVal = new List<T>();

            foreach(DiscordRoleConfiguration config in configurations)
            {
                retVal.Add(CreateFrom<T>(config,allRoles, allinvites));
            }

            return retVal;
        }
        public static T CreateFrom<T>(DiscordRoleConfiguration config, IEnumerable<DiscordRole> allRoles, IEnumerable<DiscordInvite> allinvites) where T : DiscordConfiguration, new()
        {
            return CreateFrom<T>(config, allRoles, allinvites.Index((x) => { return x.ID; }));
        }
        public static T CreateFrom<T>(DiscordRoleConfiguration config, IEnumerable<DiscordRole> allRoles, Dictionary<string, DiscordInvite> allinvites) where T : DiscordConfiguration, new()
        {
            string inviteUrl = string.Empty;
       
            if(config.GuildInviteID != null)
            {
                if (allinvites.ContainsKey(config.GuildInviteID))
                {
                    inviteUrl = allinvites[config.GuildInviteID].Url;
                }
            }
            
            return new T()
            {
                Id = config.Id,
                Name = config.Name,
                GuildInviteID = config.GuildInviteID,
                InviteUrl = inviteUrl,
                Roles = SelectedDiscordRole.CreateFrom(allRoles, config.AssignedRoles)
            };
        }

        public DiscordRoleConfiguration Create()
        {
            DiscordRoleConfiguration config = new DiscordRoleConfiguration()
            {
                Id = Id,
                Name = Name,
                GuildInviteID = GuildInviteID
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
