using Eve.ESI.Standard.DataItem;
using Eve.ESI.Standard.DataItem.Alliance;
using Eve.ESI.Standard.DataItem.Character;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.DataItem.Search;
using Eve.Static.Standard;
using Eve.Static.Standard.chr;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.Authentication.Client
{
    public class PublicDataProvider
    {
        public IStaticDataCache Cache { get; }
        public IESIAuthenticationClient Client { get; }
        public ICommandController Controller { get; }

        private readonly ICommandController m_tenantController;
        public PublicDataProvider(IESIAuthenticationClient client, ICommandController publicDataController, IStaticDataCache cache,ICommandController tenantController)
        {
            Client = client;
            Controller = publicDataController;
            Cache = cache;
            m_tenantController = tenantController;
        }

        public Task<ESICallResponse<CharacterInfo>> GetCharacterInfo(long characterID, bool oldData = false)
        {
            return CharacterInfo.GetCharacterInfo(Client, Controller, characterID, oldData);
        }
        public Task<ESICallResponse<AllianceInfo>> GetAllianceInfo(int allianceID, bool oldData = false)
        {
            return AllianceInfo.GetAllianceInfo(Client, Controller, allianceID, oldData);
        }
        public Task<ESICallResponse<CorporationInfo>> GetCorporationInfo(int corporationID, bool oldData = false)
        {
            return CorporationInfo.GetCorporationInfo(Client, Controller, corporationID, oldData);
        }

        public async Task<string> GetEntityName(eESIEntityType entityType, long entityID)
        {
            string retVal = string.Empty;

            switch (entityType)
            {
                case eESIEntityType.character:
                    {
                        ESICallResponse<CharacterInfo> info = await GetCharacterInfo(entityID, true);
                        if (info.HasData)
                        {
                            retVal = info.Data.Name;
                        }
                    }
                    break;
                case eESIEntityType.corporation:
                    {
                        ESICallResponse<CorporationInfo> info = await GetCorporationInfo((int)entityID, true);
                        if (info.HasData)
                        {
                            retVal = info.Data.Name;
                        }
                    }
                    break;
                case eESIEntityType.alliance:
                    {
                        ESICallResponse<AllianceInfo> info = await GetAllianceInfo((int)entityID, true);
                        if (info.HasData)
                        {
                            retVal = info.Data.Name;
                        }
                    }
                    break;
                case eESIEntityType.faction:
                    {
                        ChrFactions faction = Cache.GetItem<ChrFactions>((int)entityID);
                        if (faction != null)
                        {
                            retVal = faction.FactionName;
                        }
                    }
                    break;
                case eESIEntityType.role:
                    {
                        retVal = ((eESIRole)entityID).ToDelimitedString(suppressZero: true);
                    }
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(retVal))
            {
                retVal = "Name not found";
            }
            return retVal;
        }

        public Task<ESICallResponse<SearchResults>> Search(string query,eSearchEntity entities)
        {
            return SearchResults.Search(Client, Controller, query, entities);
        }
        
        
    }
}
