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
using Eve.ESI.Standard.Token;
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
        private readonly IEnumerable<Token.ESIToken> m_accountTokens;
        public IESIAuthenticatedConfig Config { get; }
        public ICommandController Controller { get; }
        public IStaticDataCache Cache { get; }
        public eESIEntityType EntityType { get; }
        public long EntityID { get; }

        private AuthenticatedEntity(IESIAuthenticatedConfig config, ICommandController controller, IStaticDataCache cache, IEnumerable<Token.ESIToken> tokens, long entityID, eESIEntityType entityType)
        {
            m_accountTokens = tokens;
            Config = config;
            Controller = controller;
            Cache = cache;
            EntityType = entityType;
            EntityID = entityID;
        }

        private Func<Task<ESITokenRefreshResponse>> GetAuthenticationToken(eESIScope scope)
        {
            return () => {
                List<ESIToken> tokens = Token.ESIToken.FindWithScope(m_accountTokens, scope);
                if (tokens.Count > 0)
                {
                    Task<ESITokenRefreshResponse> returnToken = null;
                    foreach (ESIToken token in tokens)
                    {
                        if (!token.RequiresRefresh)
                        {
                            returnToken = token.GetAuthenticationToken(Config, Controller);
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
                                Task<ESITokenRefreshResponse> tokenRefresher = token.GetAuthenticationToken(Config, Controller);
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
        public Task<ESICallResponse<CharacterInfo>> GetCharacterInfo(bool oldData = false)
        {
            return CharacterInfo.GetCharacterInfo(Config.Client, Controller, EntityID, oldData);
        }

        public bool HasScope(eESIScope scope)
        {
            return Token.ESIToken.FindWithScope(m_accountTokens, scope) != null;
        }

        public Task<ESICallResponse<CharacterSkillSheet>> GetSkillSheet(bool oldData)
        {
            return CharacterSkillSheet.GetCharacterSkillSheet(Config.Client, Controller, EntityID, GetAuthenticationToken(eESIScope.esi_skills_read_skills_v1), oldData);
        }
        public Task<ESICallResponse<CharacterAttributes>> GetAttributes()
        {
            return CharacterAttributes.GetAttributes(Config.Client, Controller, EntityID, GetAuthenticationToken(eESIScope.esi_skills_read_skills_v1));
        }
        public Task<ESICollectionCallResponse<SkillQueueItem>> GetSkillQueue(bool oldData)
        {
            return SkillQueueItem.GetSkillQueue(Config.Client, Controller, EntityID, GetAuthenticationToken(eESIScope.esi_skills_read_skillqueue_v1), oldData);
        }
        public Task<ESIIntegerCollectionCallResponse<ActiveImplants>> GetActiveImplants()
        {
            return ActiveImplants.GetActiveImplants(Config.Client, Controller, EntityID, GetAuthenticationToken(eESIScope.esi_clones_read_implants_v1));
        }
        public Task<ESICallResponse<CharacterRoles>> GetRoles()
        {
            return CharacterRoles.GetCharacterRoles(Config.Client, Controller, EntityID, GetAuthenticationToken(eESIScope.esi_characters_read_corporation_roles_v1));
        }
        public Task<ESICallResponse<CharacterFleet>> GetCharacterFleet(bool oldData = false)
        {
            return CharacterFleet.GetCharacterFleet(Config.Client, Controller, EntityID, GetAuthenticationToken(eESIScope.esi_fleets_read_fleet_v1), oldData);
        }
        public Task<ESICollectionCallResponse<FleetMember>> GetFleetMembers(long fleetID, bool oldData = false)
        {
            return FleetMember.GetFleetMembers(Config.Client, Controller, fleetID, GetAuthenticationToken(eESIScope.esi_fleets_read_fleet_v1), oldData);
        }

        public Task<ESICollectionCallResponse<CharacterContact>> GetCharacterContacts(bool oldData = false)
        {
            return CharacterContact.GetCharacterContacts(Config.Client, Controller, EntityID, GetAuthenticationToken(eESIScope.esi_characters_read_contacts_v1), oldData);
        }
        public Task<ESICollectionCallResponse<CorporationContact>> GetCorporationContacts(bool oldData = false)
        {
            return CorporationContact.GetCorporationContacts(Config.Client, Controller, (int)EntityID, GetAuthenticationToken(eESIScope.esi_corporations_read_contacts_v1), oldData);
        }
        public Task<ESICollectionCallResponse<AllianceContact>> GetAllianceContacts(bool oldData = false)
        {
            return AllianceContact.GetAllianceContacts(Config.Client, Controller, (int)EntityID, GetAuthenticationToken(eESIScope.esi_alliances_read_contacts_v1), oldData);
        }

        public string GetImageSource(string type, long id, int size, string fileType = "png")
        {
            return Config.GetImageSource(type, id, size, fileType);
        }
        public string GetEntityImageSource(int size)
        {
            return Config.GetImageSource(EntityType, EntityID, size);
        }

        public async Task<IEnumerable<EntityRelationship>> CalculateRelevantRelationships(bool oldData = false)
        {
            List<EntityRelationship> retVal = new List<EntityRelationship>();
            retVal.AddRange(EntityRelationship.SelectForTo(Controller, EntityID, EntityType));
            switch (EntityType)
            {
                case eESIEntityType.character:
                    {
                        ESICallResponse<CharacterInfo> characterResponse = await GetCharacterInfo(oldData);
                        if (characterResponse.HasData)
                        {
                            retVal.AddRange(EntityRelationship.SelectForTo(Controller, characterResponse.Data.CorporationId,eESIEntityType.corporation));
                            if (characterResponse.Data.AllianceId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(Controller, characterResponse.Data.AllianceId, eESIEntityType.alliance));
                            }
                            if (characterResponse.Data.FactionId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(Controller, characterResponse.Data.FactionId, eESIEntityType.faction));
                            }
                        }
                    }
                    break;
                case eESIEntityType.corporation:
                    {
                        ESICallResponse<CorporationInfo> corporationResponse = await CorporationInfo.GetCorporationInfo(Config.Client, Controller, (int)EntityID, oldData);
                        if (corporationResponse.HasData)
                        {
                            if (corporationResponse.Data.AllianceId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(Controller, corporationResponse.Data.AllianceId, eESIEntityType.alliance));
                            }
                            if (corporationResponse.Data.FactionId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(Controller, corporationResponse.Data.FactionId, eESIEntityType.faction));
                            }
                        }
                    }
                    break;
                case eESIEntityType.alliance:
                    {
                        ESICallResponse<AllianceInfo> allianceReponse = await AllianceInfo.GetAllianceInfo(Config.Client, Controller, (int)EntityID, oldData);
                        if (allianceReponse.Data.FactionId > 0)
                        {
                            if (allianceReponse.Data.FactionId > 0)
                            {
                                retVal.AddRange(EntityRelationship.SelectForTo(Controller, allianceReponse.Data.FactionId, eESIEntityType.faction));
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
                        ESICallResponse<CharacterInfo> characterResponse = await GetCharacterInfo(oldData);
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
                        ESICallResponse<CorporationInfo> corporationResponse = await CorporationInfo.GetCorporationInfo(Config.Client,Controller, (int)EntityID,oldData);
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
                        ESICallResponse<AllianceInfo> allianceReponse = await AllianceInfo.GetAllianceInfo(Config.Client, Controller, (int)EntityID, oldData);
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
                    retVal.Add(new EntityRelationship(EntityID, EntityType, (long)rolesResponse.Data.RolesEnum, eESIEntityType.role, eESIEntityRelationship.Roles));
                }

                if (rolesResponse.Data.RolesAtBaseEnum != eESIRole.None)
                {
                    retVal.Add(new EntityRelationship(EntityID, EntityType, (long)rolesResponse.Data.RolesAtBaseEnum, eESIEntityType.role, eESIEntityRelationship.RolesAtBase));
                }

                if (rolesResponse.Data.RolesAtHqEnum != eESIRole.None)
                {
                    retVal.Add(new EntityRelationship(EntityID, EntityType, (long)rolesResponse.Data.RolesAtHqEnum, eESIEntityType.role, eESIEntityRelationship.RolesAtHQ));
                }

                if (rolesResponse.Data.RolesAtOtherEnum != eESIRole.None)
                {
                    retVal.Add(new EntityRelationship(EntityID, EntityType, (long)rolesResponse.Data.RolesAtOtherEnum, eESIEntityType.role, eESIEntityRelationship.RolesAtOther));
                }
            }
            return retVal;
        }

        public static List<AuthenticatedEntity> GetForAccounts(IESIAuthenticatedConfig config,ICommandController controller,IStaticDataCache cache,IEnumerable<UserAccount> accounts)
        {
            List<AuthenticatedEntity> retVal = new List<AuthenticatedEntity>();
            foreach(UserAccount account in accounts)
            {
                retVal.AddRange(GetForAccount(config, controller, cache, account));
            }
            return retVal;
        }
        public static List<AuthenticatedEntity> GetForAccount(IESIAuthenticatedConfig config,ICommandController controller,IStaticDataCache cache,UserAccount account)
        {
            return FromTokens(config, controller, cache, account?.GetTokens(controller));
        }
        public static AuthenticatedEntity FromToken(IESIAuthenticatedConfig config,ICommandController controller,IStaticDataCache cache,ESIToken token)
        {
            return new AuthenticatedEntity(config,controller,cache,new List<ESIToken>() { token },token.EntityID,token.EntityType);
        }
        public static List<AuthenticatedEntity> FromTokens(IESIAuthenticatedConfig config, ICommandController controller, IStaticDataCache cache,IEnumerable<ESIToken> tokens)
        {
            List<AuthenticatedEntity> retVal = new List<AuthenticatedEntity>();
            if(tokens != null)
            {
                Dictionary<(long entityID, eESIEntityType entityType), List<Token.ESIToken>> groupedTokens = new Dictionary<(long, eESIEntityType), List<Token.ESIToken>>();
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
                    retVal.Add(new AuthenticatedEntity(config, controller, cache, groupedTokens[key], key.entityID, key.entityType));
                }
            }
            return retVal;
        }
        public static List<AuthenticatedEntity> GetForAccount(IESIAuthenticatedConfig config, ICommandController controller,IStaticDataCache cache,string accountGuid)
        {
            return GetForAccount(config, controller, cache, UserAccount.ForGuid(controller,accountGuid));
        }
        public static AuthenticatedEntity GetForTenant(IESIAuthenticatedConfig config,ICommandController controller,IStaticDataCache cache,Tenant tenant)
        {
            return GetForEntity(config, controller, cache, tenant.EntityId, (eESIEntityType)tenant.EntityType);
        }
        public static AuthenticatedEntity GetForEntity(IESIAuthenticatedConfig config, ICommandController controller, IStaticDataCache cache,long entityID,eESIEntityType entityType)
        {
            AuthenticatedEntity retVal = null;
            List<AuthenticatedEntity> authEntities = FromTokens(config, controller, cache, ESIToken.ForEntityTypeAndID(controller, entityID, entityType));
            if(authEntities.Count > 0)
            {
                retVal = authEntities[0];
            }
            return retVal;
        }
        public static async Task<List<AuthenticatedEntity>> GetRelevantForEntityType(IESIAuthenticatedConfig config, ICommandController controller, IStaticDataCache cache, string accountGuid,eESIEntityType type,long entityID)
        {
            List<AuthenticatedEntity> all = GetForAccount(config, controller, cache, accountGuid);
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
                            ESICallResponse<CharacterInfo> info = await character.GetCharacterInfo();
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
        public static async Task<string> GetEntityName(IESIAuthenticationClient client, ICommandController controller,IStaticDataCache cache, eESIEntityType entityType,long entityID)
        {
            string retVal = string.Empty;

            switch (entityType)
            {
                case eESIEntityType.character:
                    {
                        ESICallResponse<CharacterInfo> info = await CharacterInfo.GetCharacterInfo(client, controller, entityID, true);
                        if (info.HasData)
                        {
                            retVal = info.Data.Name;
                        }
                    }
                    break;
                case eESIEntityType.corporation:
                    {
                        ESICallResponse<CorporationInfo> info = await CorporationInfo.GetCorporationInfo(client, controller, (int)entityID, true);
                        if (info.HasData)
                        {
                            retVal = info.Data.Name;
                        }
                    }
                    break;
                case eESIEntityType.alliance:
                    {
                        ESICallResponse<AllianceInfo> info = await AllianceInfo.GetAllianceInfo(client, controller, (int)entityID, true);
                        if (info.HasData)
                        {
                            retVal = info.Data.Name;
                        }
                    }
                    break;
                case eESIEntityType.faction:
                    {
                        ChrFactions faction = cache.GetItem<ChrFactions>((int)entityID);
                        if(faction != null)
                        {
                            retVal = faction.FactionName;
                        }
                    }
                    break;
                case eESIEntityType.role:
                    {
                        retVal = ((eESIRole)entityID).ToDelimitedString();
                    }
                    break;
                default:
                    break;
            }

            return retVal;
        }
        
    }
}
