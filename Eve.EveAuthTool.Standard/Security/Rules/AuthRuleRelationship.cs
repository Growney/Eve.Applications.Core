using Eve.ESI.Standard;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Security.Rules
{
    public class AuthRuleRelationship : StoredObjectBase
    {
        public override long Id { get; set; }
        public long RuleID { get; set; }
        public long EntityID { get; set; }
        public eESIEntityType EntityType { get; set; }
        public eESIEntityRelationship Relationship { get; set; }

        protected override void AddParametersToSave(IDataCommand command)
        {
            command.AddParameter("RuleID", System.Data.DbType.Int64).Value = RuleID;
            command.AddParameter("EntityID", System.Data.DbType.Int64).Value = EntityID;
            command.AddParameter("EntityType", System.Data.DbType.Int32).Value = (int)EntityType;
            command.AddParameter("Relationship", System.Data.DbType.Int32).Value = (int)Relationship;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            RuleID = adapter.GetValue("RuleID", 0L);
            EntityID = adapter.GetValue("EntityID", 0L);
            EntityType = (eESIEntityType)adapter.GetValue("EntityType", 0);
            Relationship = (eESIEntityRelationship)adapter.GetValue("Relationship", 0);
        }

        public static List<AuthRuleRelationship> ForRule(ICommandController controller,long ruleID)
        {
            DataCommand command = new DataCommand("AuthRuleRelationship", "ForRule");
            command.AddParameter("RuleID", System.Data.DbType.Int64).Value = ruleID;

            return Load<AuthRuleRelationship>(controller.ExecuteCollectionCommand(command));
        }
        public static AuthRuleRelationship GetStaticRelationship(long entityID,eESIEntityType entityType)
        {
            return new AuthRuleRelationship() { EntityID = entityID, EntityType = entityType, Relationship = GetStaticRelationship(entityType) };
        }
        private static eESIEntityRelationship GetStaticRelationship(eESIEntityType entityType)
        {
            eESIEntityRelationship retVal = eESIEntityRelationship.None;
            switch (entityType)
            {
                case eESIEntityType.character:
                    retVal = eESIEntityRelationship.Character;
                    break;
                case eESIEntityType.corporation:
                    retVal = eESIEntityRelationship.Corporation;
                    break;
                case eESIEntityType.alliance:
                    retVal = eESIEntityRelationship.Alliance;
                    break;
                case eESIEntityType.faction:
                    retVal = eESIEntityRelationship.Faction;
                    break;
                default:
                    break;
            }
            return retVal;
        }
    }
}
