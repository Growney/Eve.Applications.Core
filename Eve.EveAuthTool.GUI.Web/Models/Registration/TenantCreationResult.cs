using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Registration
{
    public class TenantCreationResult
    {
        public bool IsSuccess { get; set; }
        public int Validation { get; set; }
        public string RedirectUrl { get; set; }
    }
}
