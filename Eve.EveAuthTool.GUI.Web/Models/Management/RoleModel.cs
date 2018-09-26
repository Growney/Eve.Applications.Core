using Eve.EveAuthTool.Standard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Management
{
    public class RoleModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Register { get; set; }
        public bool Manage { get; set; }
        
        public static RoleModel CreateFrom(Eve.EveAuthTool.Standard.Security.Rules.Role role)
        {
            return new RoleModel()
            {
                Id = role.Id,
                Name = role.Name,
                Register = role.Permissions.HasFlag(eRulePermission.Register),
                Manage = role.Permissions.HasFlag(eRulePermission.Manage)
            };
        }

        public static List<RoleModel> CreateFrom(IEnumerable<Eve.EveAuthTool.Standard.Security.Rules.Role> roles)
        {
            List<RoleModel> retVal = new List<RoleModel>();

            foreach(var role in roles)
            {
                retVal.Add(CreateFrom(role));
            }

            return retVal;
        }
    }
}
