using Eve.ESI.Standard.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Registration
{
    public class RegistrationFormOptions
    {
        public string Action { get; set; }
        public string Controller { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public ScopeGroup[] Groups { get; set; }
    }
}
