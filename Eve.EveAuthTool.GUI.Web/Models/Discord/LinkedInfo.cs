using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Discord
{
    public class LinkedInfo
    {
        public string GuildName { get; set; }
        public List<DiscordConfiguration> Configurations { get; set; }
    }
}
