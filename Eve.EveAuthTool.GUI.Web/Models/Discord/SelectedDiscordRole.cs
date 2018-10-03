using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Discord
{
    public class SelectedDiscordRole : DiscordRole
    {
        public bool Selected { get; set; }

        public static List<SelectedDiscordRole> CreateFrom(IEnumerable<DiscordRole> allRoles,HashSet<ulong> selected)
        {
            List<SelectedDiscordRole> roles = new List<SelectedDiscordRole>();
            foreach(DiscordRole role in allRoles)
            {
                roles.Add(new SelectedDiscordRole()
                {
                    ID = role.ID,
                    Name = role.Name,
                    Selected = selected.Contains(role.ID)
                });
            }
            return roles;
        }
    }
}
