using Eve.EveAuthTool.Standard.Security.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Management
{
    public class IndexAuthRuleRelationship : AuthRuleRelationship
    {
        public int Index { get; set; }
        
        public IndexAuthRuleRelationship(int index, AuthRuleRelationship relationship)
        {
            Id = relationship.Id;
            RuleID = relationship.RuleID;
            EntityID = relationship.EntityID;
            EntityType = relationship.EntityType;
            Relationship = relationship.Relationship;
            Index = index;
        }
    }
}
