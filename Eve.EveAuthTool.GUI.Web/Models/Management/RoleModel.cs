using Eve.EveAuthTool.Standard.Security;
using Gware.Standard.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Management
{
    public class RoleModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Register { get; set; }
        public bool Manage { get; set; }
        public long DiscordRoleConfigurationID { get; set; }
        public string DiscordRoleConfigurationName { get; set; }


        public static T CreateFrom<T>(Eve.EveAuthTool.Standard.Security.Rules.Role role, Standard.Discord.Configuration.Tenant.DiscordRoleConfiguration config) where T : RoleModel,new()
        {
            return new T()
            {
                Id = role.Id,
                Name = role.Name,
                Register = role.Permissions.HasFlag(eRulePermission.Register),
                Manage = role.Permissions.HasFlag(eRulePermission.Manage),
                DiscordRoleConfigurationID = role.DiscordRoleConfigurationID,
                DiscordRoleConfigurationName = config?.Name ?? "None"
            };
        }

        public static List<RoleModel> CreateFrom(IEnumerable<Eve.EveAuthTool.Standard.Security.Rules.Role> roles,IEnumerable<Standard.Discord.Configuration.Tenant.DiscordRoleConfiguration> discordConfigs)
        {

            Dictionary<long, Standard.Discord.Configuration.Tenant.DiscordRoleConfiguration> groupedConfigs = discordConfigs.Index( (x) => { return x.Id; });

            List<RoleModel> retVal = new List<RoleModel>();

            foreach(var role in roles)
            {
                retVal.Add(CreateFrom<RoleModel>(role,groupedConfigs.Get(role.DiscordRoleConfigurationID)));
            }

            return retVal;
        }
    }
}
