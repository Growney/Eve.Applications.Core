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
                m_cache.Add((relationship.ToEntityID, relationship.ToEntityType, relationship.Relationship), relationship);
            }
        }
        
        public bool TryGet(long entityID,eESIEntityType entityType,eESIEntityRelationship relationshipType,out EntityRelationship relationship)
        {
            return m_cache.TryGetValue((entityID, entityType, relationshipType), out relationship);
        }
        public bool TryGet(AuthRuleRelationship rule,out EntityRelationship relationship)
        {
            return TryGet(rule.EntityID, rule.EntityType, rule.Relationship, out relationship);
        }
    }
}
