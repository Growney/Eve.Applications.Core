using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.DataItem.Corporation;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Security.Rules
{
    public class AuthRuleRelationship : StoredObjectBase
    {
        public override long Id { get; set; }
        public long RuleID { get; set; }
        public long EntityID { get; set; }
        public eESIEntityType EntityType { get; set; }
        public eESIEntityRelationship Relationship { get; set; }
        public long ParentEntityID { get; set; } = 0;
        public eESIEntityType ParentEntityType { get; set; } = eESIEntityType.none;

        protected override void AddParametersToSave(IDataCommand command)
        {
            command.AddParameter("RuleID", System.Data.DbType.Int64).Value = RuleID;
            command.AddParameter("EntityID", System.Data.DbType.Int64).Value = EntityID;
            command.AddParameter("EntityType", System.Data.DbType.Int32).Value = (int)EntityType;
            command.AddParameter("Relationship", System.Data.DbType.Int32).Value = (int)Relationship;
            command.AddParameter("ParentEntityID", System.Data.DbType.Int64).Value = ParentEntityID;
            command.AddParameter("ParentEntityType", System.Data.DbType.Int32).Value = (int)ParentEntityType;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            RuleID = adapter.GetValue("RuleID", 0L);
            EntityID = adapter.GetValue("EntityID", 0L);
            EntityType = (eESIEntityType)adapter.GetValue("EntityType", 0);
            Relationship = (eESIEntityRelationship)adapter.GetValue("Relationship", 0);
            ParentEntityID = adapter.GetValue("ParentEntityID", 0L);
            ParentEntityType = (eESIEntityType)adapter.GetValue("ParentEntityType", 0);
        }

        public static void DeleteForRule(ICommandController controller, long ruleID)
        {
            DataCommand command = new DataCommand("AuthRuleRelationship", "DeleteForRule");
            command.AddParameter("RuleID", System.Data.DbType.Int64).Value = ruleID;

            controller.ExecuteQuery(command);
        }

        public static List<AuthRuleRelationship> ForRule(ICommandController controller, long ruleID)
        {
            DataCommand command = new DataCommand("AuthRuleRelationship", "ForRule");
            command.AddParameter("RuleID", System.Data.DbType.Int64).Value = ruleID;

            return Load<AuthRuleRelationship>(controller.ExecuteCollectionCommand(command));
        }
        public static AuthRuleRelationship GetRelationshipRule(long entityID, eESIEntityType entityType, eESIEntityRelationship relationship, long parentEntityID, eESIEntityType parentEntityType)
        {
            return new AuthRuleRelationship() { EntityID = entityID, EntityType = entityType, Relationship = relationship, ParentEntityID = parentEntityID, ParentEntityType = parentEntityType };
        }
        public static AuthRuleRelationship GetRelationshipRule(long entityID, eESIEntityType entityType, eESIEntityRelationship relationship)
        {
            return new AuthRuleRelationship() { EntityID = entityID, EntityType = entityType, Relationship = relationship };
        }
        public static AuthRuleRelationship GetStaticRelationship(long entityID, eESIEntityType entityType)
        {
            return GetRelationshipRule(entityID, entityType, GetStaticRelationship(entityType));
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

        public async Task<string> GetRelationshipEntityName(ICommandController tenantController,PublicDataProvider publicData)
        {
            string retVal = string.Empty;
            switch (EntityType)
            {
                case eESIEntityType.title:
                    {
                        CorporationTitle title = CorporationTitle.ForTitleID(tenantController, (int)ParentEntityID, (int)EntityID);
                        if (title != null)
                        {
                            ESICallResponse<CorporationInfo> corpInfo = await publicData.GetCorporationInfo((int)ParentEntityID, true);
                            if (corpInfo.HasData)
                            {
                                retVal = $"{corpInfo.Data.Ticker} - {title.Name}";
                            }
                            else
                            {
                                retVal = title.Name;
                            }
                        }
                    }
                    break;
                default:
                    retVal = await publicData.GetEntityName(EntityType, EntityID);
                    break;
            }
            return retVal;
        }
    }
}
