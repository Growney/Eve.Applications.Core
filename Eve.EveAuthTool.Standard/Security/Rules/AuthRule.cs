using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.DataItem;
using Eve.ESI.Standard.Relationships;
using Eve.Static.Standard;
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
        public bool MatchAll { get; set; }
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

            command.AddParameter("MatchAll", System.Data.DbType.Boolean).Value = MatchAll;
        }
        protected override void OnSave(ICommandController controller)
        {
            AuthRuleRelationship.DeleteForRule(controller, Id);
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
            MatchAll = adapter.GetValue("MatchAll", true);
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
        public static async Task<Role> GetEntityRole(IESIAuthenticatedConfig config, ICommandController tenantcontroller, IStaticDataCache staticCache, PublicDataProvider publicData, List<AuthRule> inOrderRules,IEnumerable<EntityRelationship> queryRelationships)
        {
            long roleID = 0;
            EntityRelationshipCache cache = new EntityRelationshipCache(queryRelationships);
            foreach (AuthRule rule in inOrderRules)
            {
                bool matched = rule.MatchAll && rule.Relationships.Count > 0;
                foreach (AuthRuleRelationship relationship in rule.Relationships)
                {
                    bool found = false;
                    if (EntityRelationship.MeetsMask(relationship.Relationship, eESIEntityRelationshipOperatorMask.HasFrom))
                    {
                        AuthenticatedEntity ruleRelationshipEntity = AuthenticatedEntity.GetForEntity(config, tenantcontroller, staticCache, publicData, relationship.EntityID, relationship.EntityType);
                        if(ruleRelationshipEntity != null)
                        {
                            IEnumerable<EntityRelationship> ruleEntityRelationships = await ruleRelationshipEntity.CalculateInRelationships(relationship.Relationship);
                            EntityRelationshipCache ruleEntityCache = new EntityRelationshipCache(ruleEntityRelationships);
                            foreach(EntityRelationship queryRelationship in queryRelationships)
                            {
                                if (EntityRelationship.MeetsMask(queryRelationship.Relationship, eESIEntityRelationshipOperatorMask.In))
                                {
                                    if(ruleEntityCache.TryGet(queryRelationship.ToEntityID,queryRelationship.ToEntityType,relationship.Relationship,out _))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        found = cache.TryGet(relationship, out EntityRelationship _);
                    }
                     
                    if (rule.MatchAll)
                    {
                        if (!found)
                        {
                            matched = false;
                            break;
                        }
                    }
                    else
                    {
                        if (found)
                        {
                            matched = true;
                            break;
                        }
                    }
                }
                if (matched)
                {
                    roleID = rule.RoleId;
                    break;
                }
            }
            if (roleID > 0)
            {
                return Role.Get<Role>(tenantcontroller,roleID);

            }
            else
            {
                return null;
            }
            
        }

        public static async Task<Role> GetEntityRole(IESIAuthenticatedConfig config, ICommandController tenantcontroller, IStaticDataCache staticCache, PublicDataProvider publicData, AuthenticatedEntity entity)
        {
            return await GetEntityRole(config, tenantcontroller, staticCache,publicData,InOrder(tenantcontroller),await entity.CalculateRelevantRelationships(false));
            
        }

        public static AuthRule CreateAdminRule(ICommandController controller,int corporationID)
        {
            Role allRole = new Role()
            {   Name = "Corporation Directors",
                Permissions = eRulePermission.All,
                Ordinal = 0
                
            };
            AuthRule retVal = new AuthRule()
            {
                Name = "Corporation Directors",
                RoleId = allRole.Save(controller),
                Ordinal = 0,
                MatchAll = true

            }.AddRelationship(AuthRuleRelationship.GetStaticRelationship(corporationID, eESIEntityType.corporation))
            .AddRelationship(AuthRuleRelationship.GetRelationshipRule((int)eESIRole.Director, eESIEntityType.role, eESIEntityRelationship.Roles));
            retVal.Save(controller);

            return retVal;
        }

        public static AuthRule CreateMemberRule(ICommandController controller,long entityID,eESIEntityType entityType)
        {
            Role allRole = new Role()
            {
                Name = "Members",
                Permissions = eRulePermission.Discord,
                Ordinal = 1
            };
            AuthRule retVal = new AuthRule()
            {
                Name = "Members",
                RoleId = allRole.Save(controller),
                Ordinal = 1,
                MatchAll = true

            }.AddRelationship(AuthRuleRelationship.GetStaticRelationship(entityID, entityType));
            retVal.Save(controller);
            return retVal;
        }

        public static AuthRule CreateStandingRule(ICommandController controller, long entityID, eESIEntityType entityType)
        {
            Role allRole = new Role()
            {
                Name = "Blues",
                Permissions = eRulePermission.Discord,
                Ordinal = 1
            };
            AuthRule retVal = new AuthRule()
            {
                Name = "Blues",
                RoleId = allRole.Save(controller),
                Ordinal = 1,
                MatchAll = false

            }.AddRelationship(AuthRuleRelationship.GetRelationshipRule(entityID, entityType, eESIEntityRelationship.GoodStanding))
            .AddRelationship(AuthRuleRelationship.GetRelationshipRule(entityID, entityType, eESIEntityRelationship.ExcellentStanding));
            retVal.Save(controller);
            return retVal;
        }
    }
}
