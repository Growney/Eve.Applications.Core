using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Registration
{
    public class GroupRegistrationOptions
    {
        public uint[] SelectedScopes { get; set; }
        public bool RegisterCharacter { get; set; }
    }
}
