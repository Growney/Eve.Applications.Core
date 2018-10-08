using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Management
{
    public class EditRoleModel : RoleModel
    {
        public List<Models.Discord.DiscordConfiguration> AllConfigurations { get; set; }
    }
}
