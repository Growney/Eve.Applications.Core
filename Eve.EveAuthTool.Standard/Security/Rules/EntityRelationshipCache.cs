using Eve.ESI.Standard;
using Eve.ESI.Standard.Relationships;
using System.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Security.Rules
{
    internal class EntityRelationshipCache
    {
        private Dictionary<(long, eESIEntityType, eESIEntityRelationship), EntityRelationship> m_cache = new Dictionary<(long, eESIEntityType, eESIEntityRelationship), EntityRelationship>();
        public EntityRelationshipCache(IEnumerable<EntityRelationship> relationships)
        {
            foreach(EntityRelationship relationship in relationships)
            {
                if(MeetsMask(relationship.Relationship,eESIEntityRelationshipOperatorMask.In))
                {
                    m_cache.Add((relationship.FromEntityID, relationship.FromEntityType, relationship.Relationship), relationship);
                }
                else if(MeetsMask(relationship.Relationship,eESIEntityRelationshipOperatorMask.Has) 
                    || MeetsMask(relationship.Relationship, eESIEntityRelationshipOperatorMask.Is))
                {
                    m_cache.Add((relationship.ToEntityID, relationship.ToEntityType, relationship.Relationship), relationship);
                }
            }
        }
        private static bool MeetsMask(eESIEntityRelationship relationship,eESIEntityRelationshipOperatorMask mask)
        {
            return ((int)relationship / (int)mask) == 1;
        }

        public bool TryGet(AuthRuleRelationship rule,out EntityRelationship relationship)
        {
            return m_cache.TryGetValue((rule.EntityID, rule.EntityType, rule.Relationship), out relationship);
        }
    }
}
