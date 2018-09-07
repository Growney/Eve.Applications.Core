using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.Relationships;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.EveAuthTool.Standard.Security.Rules
{
    public class AuthRule : StoredObjectBase
    {
        public override long Id { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public int Ordinal { get; set; }
        public List<AuthRuleRelationship> Relationships { get; }

        public AuthRule()
        {
            Relationships = new List<AuthRuleRelationship>();
        }

        protected override void AddParametersToSave(IDataCommand command)
        {
            command.AddParameter("Name", System.Data.DbType.String).Value = Name ?? String.Empty;
            command.AddParameter("RoleID", System.Data.DbType.Int64).Value = RoleId;
            command.AddParameter("Ordinal", System.Data.DbType.Int32).Value = Ordinal;
        }
        protected override void OnSave(ICommandController controller)
        {
            Relationships.Save(controller,x=>
            {
                x.RuleID = Id;
            });
            base.OnSave(controller);
        }
        public AuthRule AddRelationship(AuthRuleRelationship relationship)
        {
            relationship.RuleID = Id;
            Relationships.Add(relationship);
            return this;
        }
        protected override void OnLoad(IDataAdapter adapter)
        {
            Name = adapter.GetValue("Name", string.Empty);
            RoleId = adapter.GetValue("RoleID", 0L);
            Ordinal = adapter.GetValue("Ordinal", 0);
            Relationships.AddRange(AuthRuleRelationship.ForRule(adapter.Controller, Id));

        }
        public static List<AuthRule> InOrder(ICommandController controller)
        {
            List<AuthRule> retVal = All(controller);
            retVal.Sort((x, y) => x.Ordinal.CompareTo(y.Ordinal));
            return retVal;
        }
        public static List<AuthRule> All(ICommandController controller)
        {
            DataCommand command = new DataCommand("AuthRule", "All");
            return Load<AuthRule>(controller.ExecuteCollectionCommand(command));
        }
        public static bool TryEntityRoleID(List<AuthRule> inOrderRules,IEnumerable<EntityRelationship> relationships,out long roleID)
        {
            bool retVal = false;
            roleID = 0;
            EntityRelationshipCache cache = new EntityRelationshipCache(relationships);
            foreach (AuthRule rule in inOrderRules)
            {
                bool matched = true;
                foreach (AuthRuleRelationship relationship in rule.Relationships)
                {
                    if (!cache.TryGet(relationship, out EntityRelationship _))
                    {
                        matched = false;
                        break;
                    }
                }
                if (matched)
                {
                    retVal = true;
                    roleID = rule.RoleId;
                    break;
                }
            }
            return retVal;
        }
        public static Role GetEntityRole(ICommandController controller,IEnumerable<EntityRelationship> relationships)
        {
            Role retVal = null;
            if (TryEntityRoleID(InOrder(controller),relationships, out long roleID))
            {
                return Role.Get<Role>(controller, roleID);
            }
            return retVal;
        }

        public static async Task<Role> GetEntityRole(ICommandController controller,AuthenticatedEntity entity)
        {
            return GetEntityRole(controller,await entity.CalculateRelevantRelationships(false));
            
        }

        public static AuthRule CreateDefaultRole(ICommandController controller,long entityID,eESIEntityType entityType)
        {
            Role allRole = new Role()
            {   Name = "Tenant",
                Permissions = eRulePermission.All,
                Ordinal = 0
            };
            AuthRule retVal = new AuthRule()
            {
                Name = "Default",
                RoleId = allRole.Save(controller),
                Ordinal = 0
                
            }.AddRelationship(AuthRuleRelationship.GetStaticRelationship(entityID, entityType));
            retVal.Save(controller);

            return retVal;
        }
    }
}
