using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Discord
{
    public class EditDiscordConfiguration : DiscordConfiguration
    {
        public List<DiscordInvite> Invites { get; set; }
    }
}
