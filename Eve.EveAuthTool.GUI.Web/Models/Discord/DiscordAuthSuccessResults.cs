using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Discord
{
    public class DiscordAuthSuccessResults
    {
        public bool UpdatedNickname { get; set; }
        public string UpdatedNicknameTo { get; set; }

        public int RolesAdded { get; set; }
        public int RolesRemoved { get; set; }

        public int RolesShouldHaveBeenAdded { get; set; }
        public int RolesShouldHaveBeenRemoved { get; set; }

        public int RolesFailedToAdd
        {
            get
            {
                return RolesShouldHaveBeenAdded - RolesAdded;
            }
        }
        public int RolesFailedToRemove
        {
            get
            {
                return RolesShouldHaveBeenRemoved - RolesRemoved;
            }
        }
    }
}
