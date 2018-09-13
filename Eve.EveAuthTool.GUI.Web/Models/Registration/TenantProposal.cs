using Eve.ESI.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Registration
{
    public class TenantProposal
    {
        public eESIEntityType EntityType { get; set; }
        public long EntityID { get; set; }
        public string TokenGuid { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
