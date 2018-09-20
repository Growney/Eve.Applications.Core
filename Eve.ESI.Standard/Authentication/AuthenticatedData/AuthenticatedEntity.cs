using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.DataItem;
using Eve.ESI.Standard.DataItem.Alliance;
using Eve.ESI.Standard.DataItem.Character;
using Eve.ESI.Standard.DataItem.Clones;
using Eve.ESI.Standard.DataItem.Contacts;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.DataItem.Fleet;
using Eve.ESI.Standard.DataItem.Skills;
using Eve.ESI.Standard.Relationships;
using Eve.ESI.Standard.Authentication.Token;
using Eve.Static.Standard;
using Eve.Static.Standard.chr;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.AuthenticatedData
{
    public class AuthenticatedEntity
    {
        private readonly IEnumerable<ESIToken> m_accountTokens;
        public IESIAuthenticatedConfig Config { get; }
        public ICommandController TenantController { get; }
        public IStaticDataCache Cache { get; }
        public eESIEntityType EntityType { get; }
        public PublicDataProvider PublicData { get; }
        public long EntityID { get; }

        private AuthenticatedEntity(IESIAuthenticatedConfig config, ICommandController tenantController, IStaticDataCache cache, IEnumerable<ESIToken> tokens, PublicDataProvider publicData, long entityID, eESIEntityType entityType)
        {
            m_accountTokens = tokens;
            Config = config;
            TenantController = tenantController;
            Cache = cache;
            EntityType = entityType;
            EntityID = entityID;
            PublicData = publicData;
        }

        private Func<Task<ESITokenRefreshResponse>> GetAuthenticationToken(eESIScope scope)
        {
            return () => {
                List<ESIToken> tokens = ESIToken.FindWithScope(m_accountTokens, scope);
                if (tokens.Count > 0)
                {
                    Task<ESITokenRefreshResponse> returnToken = null;
                    foreach (ESIToken token in tokens)
                    {
                        if (!token.RequiresRefresh)
                        {
                            returnToken = token.GetAuthenticationToken(Config, TenantController);
                            break;
                        }
                    }
                    if (returnToken == null)
                    {
                        returnToken = Task<ESITokenRefreshResponse>.Factory.StartNew(() =>
                        {
                            ESITokenRefreshResponse tokenResponse = new ESITokenRefreshResponse(eESITokenRefreshResponseStatus.NoTokenAvaliable);
                            foreach (ESIToken token in tokens)
                            {
                                Task<ESITokenRefreshResponse> tokenRefresher = token.GetAuthenticationToken(Config, TenantController);
                                tokenRefresher.Wait();
                                if (tokenRefresher.IsCompleted)
                                {
                                    if (tokenRefresher.Result.Status == eESITokenRefreshResponseStatus.Success)
                                    {
                                        tokenResponse = tokenRefresher.Result;
                                        break;
                                    }
                                }

                            }

                            return tokenResponse;
                        });
                    }
                    return returnToken;
                }
                else
                {
                    TaskCompletionSource<ESITokenRefreshResponse> result = new TaskCompletionSource<ESITokenRefreshResponse>();

                    result.SetResult(new ESITokenRefreshResponse(eESITokenRefreshResponseStatus.NoScopeFound));

                    return result.Task;
                }

            };

        }
        public bool HasScope(eESIScope scope)
        {
            return ESIToken.FindWithScope(m_accountTokens, scope) != null;
        }

        public Task<ESICallResponse<CharacterSkillSheet>> GetSkillSheet(bool oldData)
        {
            return CharacterSkillSheet.GetCharacterSkillSheet(Config.Client, TenantController, EntityID, GetAuthenticationToken(eESIScope.esi_skills_read_skills_v1), oldData);
        }
        public Task<ESICallResponse<CharacterAttributes>> GetAttributes()
        {
            return CharacterAttributes.GetAttributes(Config.Client, TenantController, EntityID, GetAuthenticationToken(eESIScope.esi_skills_read_skills_v1));
        }
        public Task<ESICollectionCallResponse<SkillQueueItem>> GetSkillQueue(bool oldData)
        {
            return SkillQueueItem.GetSkillQueue(Config.Client, TenantController, EntityID, GetAuthenticationToken(eESIScope.esi_skills_read_skillqueue_v1), oldData);
        }
        public Task<ESIIntegerCollectionCallResponse<ActiveImplants>> GetActiveImplants()
        {
            return ActiveImplants.GetActiveImplants(Config.Client, TenantController, EntityID, GetAuthenticationToken(eESIScope.esi_clones_read_implants_v1));
        }
        public Task<ESICallResponse<CharacterRoles>> GetRoles()
        {
            return CharacterRoles.GetCharacterRoles(Config.Client, TenantController, EntityID, GetAuthenticationToken(eESIScope.esi_characters_read_corporation_roles_v1));
        }
        public Task<ESICallResponse<CharacterFleet>> GetCharacterFleet(bool oldData = false)
        {
            return CharacterFleet.GetCharacterFleet(Config.Client, TenantController, EntityID, GetAuthenticationToken(eESIScope.esi_fleets_read_fleet_v1), oldData);
        }
        public Task<ESICollectionCallResponse<FleetMember>> GetFleetMembers(long fleetID, bool oldData = false)
        {
            return FleetMember.GetFleetMembers(Config.Client, TenantController, fleetID, GetAuthenticationToken(eESIScope.esi_fleets_read_fleet_v1), oldData);
        }

        public Task<ESICollectionCallResponse<CharacterContact>> GetCharacterContacts(bool oldData = false)
        {
            return CharacterContact.GetCharacterContacts(Config.Client, TenantController, EntityID, GetAuthenticationToken(eESIScope.esi_characters_read_contacts_v1), oldData);
        }
        public Task<ESICollectionCallResponse<CorporationContact>> GetCorporationContacts(bool oldData = false)
        {
            return CorporationContact.GetCorporationContacts(Config.Client, TenantController, (int)EntityID, GetAuthenticationToken(eESIScope.esi_corporations_read_contacts_v1), oldData);
        }
        public Task<ESICollectionCallResponse<AllianceContact>> GetAllianceContacts(bool oldData = false)
        {
            return AllianceContact.GetAllianceContacts(Config.Client, TenantController, (int)EntityID, GetAuthenticationToken(eESIScope.esi_alliances_read_contacts_v1), oldData);
        }

        public async Task<IEnumerable<EntityRelationship>> CalculateRelevantRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();
            retVal.AddRange(EntityRelationship.SelectForTo(TenantController, EntityID, EntityType));
            switch (EntityType)
            {
                case eESIEntityType.character:
                    {
                        ESICallResponse<CharacterInfo> characterResponse = await PublicData.GetCharacterInfo((int)EntityID,oldData);
                        if (characterResponse.HasData)
                        {
                            retVal.AddRange(EntityRelationship.SelectForTo(TenantController, characterResponse.Data.CorporationId,eESIEntityType.corporation));
                            if (characterResponse.Data.AllianceId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(TenantController, characterResponse.Data.AllianceId, eESIEntityType.alliance));
                            }
                            if (characterResponse.Data.FactionId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(TenantController, characterResponse.Data.FactionId, eESIEntityType.faction));
                            }
                        }
                    }
                    break;
                case eESIEntityType.corporation:
                    {
                        ESICallResponse<CorporationInfo> corporationResponse = await PublicData.GetCorporationInfo((int)EntityID,oldData);
                        if (corporationResponse.HasData)
                        {
                            if (corporationResponse.Data.AllianceId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(TenantController, corporationResponse.Data.AllianceId, eESIEntityType.alliance));
                            }
                            if (corporationResponse.Data.FactionId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(TenantController, corporationResponse.Data.FactionId, eESIEntityType.faction));
                            }
                        }
                    }
                    break;
                case eESIEntityType.alliance:
                    {
                        ESICallResponse<AllianceInfo> allianceReponse = await PublicData.GetAllianceInfo((int)EntityID, oldData);
                        if (allianceReponse.Data.FactionId > 0)
                        {
                            if (allianceReponse.Data.FactionId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(TenantController, allianceReponse.Data.FactionId, eESIEntityType.faction));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            
            retVal.AddRange(await CalculateHasIsRelationships());

            return retVal;
        }
        public async Task<IEnumerable<EntityRelationship>> CalculateInRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();

            retVal.AddRange(await CalculateFleetRelationships(oldData));
            retVal.AddRange(await CalculateStandingRelationships(oldData));

            return retVal;
        }

        public async Task<IEnumerable<EntityRelationship>> CalculateInRelationships(eESIEntityRelationship relationship,bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();

            switch (relationship)
            {
                case eESIEntityRelationship.Fleet:
                    retVal.AddRange(await CalculateFleetRelationships(oldData));
                    break;
                case eESIEntityRelationship.NeutralStanding:
                case eESIEntityRelationship.TerribleStanding:
                case eESIEntityRelationship.BadStanding:
                case eESIEntityRelationship.GoodStanding:
                case eESIEntityRelationship.ExcellentStanding:
                    retVal.AddRange(await CalculateStandingRelationships(oldData));
                    break;
                case eESIEntityRelationship.WarEnemy:
                    break;
                case eESIEntityRelationship.WarAlly:
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public async Task<IEnumerable<EntityRelationship>> CalculateHasIsRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();
            retVal.AddRange(await CalculateRoleRelationships(oldData));
#warning Calculate Title relationships 
#warning Calculate Asset relationships

            retVal.AddRange(await CalculatePropertyRelationships(oldData));

            return retVal;
        }
        public async Task<IEnumerable<FleetMember>> GetCharacterFleetMembers(bool oldData = false)
        {
            List<FleetMember> retVal = new List<FleetMember>();
            ESICallResponse<CharacterFleet> fleet = await GetCharacterFleet(oldData);
            if (fleet.HasData)
            {
                ESICollectionCallResponse<FleetMember> members = await GetFleetMembers(fleet.Data.FleetId);
                if (members.Data != null)
                {
                    retVal.AddRange(members.Data);
                }

            }
            return retVal;
        }
        public async Task<List<EntityRelationship>> CalculateFleetRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();
            if (EntityType == eESIEntityType.character)
            {
                ESICallResponse<CharacterFleet> fleet = await GetCharacterFleet(oldData);
                if (fleet.HasData)
                {
                    ESICollectionCallResponse<FleetMember> members = await GetFleetMembers(fleet.Data.FleetId);
                    if(members.Data != null)
                    {
                        foreach (FleetMember member in await GetCharacterFleetMembers(oldData))
                        {
                            retVal.Add(new EntityRelationship(EntityID, eESIEntityType.fleetFC, member.CharacterId, eESIEntityType.character, eESIEntityRelationship.Fleet));
                        }
                    }
                }
                
            }
            return retVal;
        }
        public async Task<List<EntityRelationship>> CalculateStandingRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();
            List<Contact> contacts = new List<Contact>();
            switch (EntityType)
            {
                case eESIEntityType.character:
                    {
                        var contactData = await GetCharacterContacts(oldData);
                        if (contactData.Data != null)
                        {
                            contacts.AddRange(contactData.Data);
                        }
                    }
                    break;
                case eESIEntityType.corporation:
                    {
                        var contactData = await GetCorporationContacts(oldData);
                        if (contactData.Data != null)
                        {
                            contacts.AddRange(contactData.Data);
                        }
                    }
                    break;
                case eESIEntityType.alliance:
                    {
                        var contactData = await GetAllianceContacts(oldData);
                        if (contactData.Data != null)
                        {
                            contacts.AddRange(contactData.Data);
                        }
                    }
                    break;
                default:
                    break;
            }

            foreach (Contact contact in contacts)
            {
                retVal.Add(new EntityRelationship(EntityID, EntityType, contact.ContactId, contact.ContactType, contact.GetRelationship()));
            }

            return retVal;
        }
        public async Task<List<EntityRelationship>> CalculatePropertyRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();

            switch (EntityType)
            {
                case eESIEntityType.character:
                    {
                        ESICallResponse<CharacterInfo> characterResponse = await PublicData.GetCharacterInfo(EntityID,oldData);
                        if (characterResponse.HasData)
                        {
                            retVal.Add(new EntityRelationship(EntityID, EntityType, EntityID, EntityType, eESIEntityRelationship.Character));
                            retVal.Add(new EntityRelationship(EntityID, EntityType, characterResponse.Data.CorporationId, eESIEntityType.corporation, eESIEntityRelationship.Corporation));
                            if(characterResponse.Data.AllianceId > 0)
                            {
                                retVal.Add(new EntityRelationship(EntityID, EntityType, characterResponse.Data.AllianceId, eESIEntityType.alliance, eESIEntityRelationship.Alliance));
                            }
                            if(characterResponse.Data.FactionId > 0)
                            {
                                retVal.Add(new EntityRelationship(EntityID, EntityType, characterResponse.Data.FactionId, eESIEntityType.faction, eESIEntityRelationship.Faction));
                            }
                        }
                    }
                    break;
                case eESIEntityType.corporation:
                    {
                        ESICallResponse<CorporationInfo> corporationResponse = await PublicData.GetCorporationInfo((int)EntityID,oldData);
                        if (corporationResponse.HasData)
                        {
                            if (corporationResponse.Data.AllianceId > 0)
                            {
                                retVal.Add(new EntityRelationship(EntityID, EntityType, corporationResponse.Data.AllianceId, eESIEntityType.alliance, eESIEntityRelationship.Alliance));
                            }
                            if (corporationResponse.Data.FactionId > 0)
                            {
                                retVal.Add(new EntityRelationship(EntityID, EntityType, corporationResponse.Data.FactionId, eESIEntityType.faction, eESIEntityRelationship.Faction));
                            }
                        }
                    }
                    break;
                case eESIEntityType.alliance:
                    {
                        ESICallResponse<AllianceInfo> allianceReponse = await PublicData.GetAllianceInfo((int)EntityID, oldData);
                        if (allianceReponse.Data.FactionId > 0)
                        {
                            retVal.Add(new EntityRelationship(EntityID, EntityType, allianceReponse.Data.FactionId, eESIEntityType.faction, eESIEntityRelationship.Faction));
                        }
                    }
                    break;
                default:
                    break;
            }

            return retVal;
        }
        public async Task<List<EntityRelationship>> CalculateRoleRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();

            ESICallResponse<CharacterRoles> rolesResponse = await GetRoles();
            if (rolesResponse.HasData)
            {
                if(rolesResponse.Data.RolesEnum != eESIRole.None)
                {
                    retVal.AddRange(EntityRelationship.ForRoleFlag(EntityID,eESIEntityRelationship.Roles,rolesResponse.Data.RolesEnum));
                }

                if (rolesResponse.Data.RolesAtBaseEnum != eESIRole.None)
                {
                    retVal.AddRange(EntityRelationship.ForRoleFlag(EntityID, eESIEntityRelationship.RolesAtBase, rolesResponse.Data.RolesAtBaseEnum));
                }

                if (rolesResponse.Data.RolesAtHqEnum != eESIRole.None)
                {
                    retVal.AddRange(EntityRelationship.ForRoleFlag(EntityID, eESIEntityRelationship.RolesAtHQ, rolesResponse.Data.RolesAtHqEnum));
                }

                if (rolesResponse.Data.RolesAtOtherEnum != eESIRole.None)
                {
                    retVal.AddRange(EntityRelationship.ForRoleFlag(EntityID, eESIEntityRelationship.RolesAtOther, rolesResponse.Data.RolesAtOtherEnum));
                }
            }
            return retVal;
        }
        

        public static List<AuthenticatedEntity> GetForAccounts(IESIAuthenticatedConfig config,ICommandController tenantcontroller,IStaticDataCache cache, PublicDataProvider publicData,IEnumerable<UserAccount> accounts)
        {
            List<AuthenticatedEntity> retVal = new List<AuthenticatedEntity>();
            foreach(UserAccount account in accounts)
            {
                retVal.AddRange(GetForAccount(config, tenantcontroller, cache, publicData, account));
            }
            return retVal;
        }
        public static List<AuthenticatedEntity> GetForAccount(IESIAuthenticatedConfig config, ICommandController tenantController, IStaticDataCache cache, PublicDataProvider publicData, UserAccount account)
        {
            return FromTokens(config,tenantController, cache, publicData, account?.GetTokens(tenantController));
        }
        public static AuthenticatedEntity FromToken(IESIAuthenticatedConfig config,ICommandController tenantController,IStaticDataCache cache, PublicDataProvider publicData, ESIToken token)
        {
            return new AuthenticatedEntity(config,tenantController,cache,new List<ESIToken>() { token },publicData,token.EntityID,token.EntityType);
        }
        public static List<AuthenticatedEntity> FromTokens(IESIAuthenticatedConfig config, ICommandController tenantController, IStaticDataCache cache, PublicDataProvider publicData, IEnumerable<ESIToken> tokens)
        {
            List<AuthenticatedEntity> retVal = new List<AuthenticatedEntity>();
            if(tokens != null)
            {
                Dictionary<(long entityID, eESIEntityType entityType), List<ESIToken>> groupedTokens = new Dictionary<(long, eESIEntityType), List<ESIToken>>();
                foreach (ESIToken token in tokens)
                {
                    (long entityID, eESIEntityType entityType) key = (token.EntityID, token.EntityType);
                    if (!groupedTokens.ContainsKey(key))
                    {
                        groupedTokens.Add(key, new List<ESIToken>());
                    }
                    groupedTokens[key].Add(token);

                }

                foreach ((long entityID, eESIEntityType entityType) key in groupedTokens.Keys)
                {
                    retVal.Add(new AuthenticatedEntity(config,tenantController, cache, groupedTokens[key],publicData, key.entityID, key.entityType));
                }
            }
            return retVal;
        }
        public static List<AuthenticatedEntity> GetForAccount(IESIAuthenticatedConfig config, ICommandController tenantcontroller, IStaticDataCache cache, PublicDataProvider publicData, string accountGuid)
        {
            return GetForAccount(config, tenantcontroller, cache, publicData, UserAccount.ForGuid(tenantcontroller, accountGuid));
        }
        public static AuthenticatedEntity GetForTenant(IESIAuthenticatedConfig config,ICommandController tenantcontroller, ICommandController controller,IStaticDataCache cache, PublicDataProvider publicData, Tenant tenant)
        {
            return GetForEntity(config, tenantcontroller, cache, publicData, tenant.EntityId, (eESIEntityType)tenant.EntityType);
        }
        public static AuthenticatedEntity GetForEntity(IESIAuthenticatedConfig config, ICommandController tenantcontroller, IStaticDataCache cache, PublicDataProvider publicData, long entityID,eESIEntityType entityType)
        {
            AuthenticatedEntity retVal = null;
            List<AuthenticatedEntity> authEntities = FromTokens(config, tenantcontroller, cache,publicData, ESIToken.ForEntityTypeAndID(tenantcontroller, entityID, entityType));
            if(authEntities.Count > 0)
            {
                retVal = authEntities[0];
            }
            return retVal;
        }
        public static async Task<List<AuthenticatedEntity>> GetRelevantForEntityType(IESIAuthenticatedConfig config, ICommandController tenantcontroller, IStaticDataCache cache, PublicDataProvider publicData, string accountGuid,eESIEntityType type,long entityID)
        {
            List<AuthenticatedEntity> all = GetForAccount(config, tenantcontroller, cache,publicData, accountGuid);
            List<AuthenticatedEntity> retval = new List<AuthenticatedEntity>();
            foreach (AuthenticatedEntity character in all)
            {
                switch (type)
                {
                    case eESIEntityType.character:
                        if(character.EntityID == entityID)
                        {
                            retval.Add(character);
                        }
                        break;
                    case eESIEntityType.alliance:
                    case eESIEntityType.corporation:
                        {
                            ESICallResponse<CharacterInfo> info = await publicData.GetCharacterInfo(character.EntityID);
                            if (info.HasData)
                            {
                                if(type== eESIEntityType.alliance)
                                {
                                    if(entityID == info.Data.AllianceId)
                                    {
                                        retval.Add(character);
                                    }
                                }
                                else if(type== eESIEntityType.corporation)
                                {
                                    if(entityID == info.Data.CorporationId)
                                    {
                                        retval.Add(character);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return retval;
        }
        
        
    }
}
