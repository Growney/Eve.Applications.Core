using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.DataItem;
using Eve.ESI.Standard.DataItem.Alliance;
using Eve.ESI.Standard.DataItem.Character;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.DataItem.Search;
using Eve.Static.Standard;
using Eve.Static.Standard.chr;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.Authentication.Client
{
    public class PublicDataProvider : IPublicDataProvider
    {
        public IStaticDataCache Cache { get; }
        public IESIAuthenticationClient Client { get; }
        public ICommandController Controller { get; }
        public ILogger<PublicDataProvider> Logger { get; }
        
        public PublicDataProvider(ILogger<PublicDataProvider> logger, IControllerProvider provider,IESIAuthenticatedConfig esiConfiguration, IStaticDataCache cache,IConfiguration config)
        {
            Logger = logger;
            Client = esiConfiguration.Client;
            Controller = provider.GetDefaultDataController();
            Cache = cache;
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
        public Task<ESICallResponse<SearchResults>> Search(string query, eSearchEntity entities)
        {
            return SearchResults.Search(Client, Controller, query, entities);
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
        public async Task<string> GetTaggedCharacterName(long characterID)
        {
            StringBuilder retVal = new StringBuilder();
            ESICallResponse<CharacterInfo> character = await GetCharacterInfo(characterID);
            if (character.HasData)
            {
                bool containsPrefix = false;
                if (character.Data.AllianceId > 0)
                {
                    Logger.LogTrace($"{character.Data.Name} is in alliance {character.Data.AllianceId}");
                    
                    ESICallResponse<AllianceInfo> allianceInfo = await GetAllianceInfo(character.Data.AllianceId, true);
                    if (allianceInfo.HasData)
                    {
                        Logger.LogTrace($"{character.Data.Name} is in alliance {character.Data.AllianceId} and that the name of the alliance is {allianceInfo.Data.Name} [{allianceInfo.Data.Ticker}]");
                        retVal.Append($"[{allianceInfo.Data.Ticker}]");
                        containsPrefix = true;
                    }
                    else
                    {
                        Logger.LogWarning($"Found no alliance info for alliance {character.Data.AllianceId}");
                    }
                    
                }
                Logger.LogTrace($"{character.Data.Name} is in corporation {character.Data.CorporationId}");
                
                ESICallResponse<CorporationInfo> corporationInfo = await GetCorporationInfo(character.Data.CorporationId, true);
                if (corporationInfo.HasData)
                {
                    Logger.LogTrace($"{character.Data.Name} is in corporation {character.Data.CorporationId} and that the name of the corporation is {corporationInfo.Data.Name} ([{corporationInfo.Data.Ticker}])");
                    retVal.Append($"[{corporationInfo.Data.Ticker}]");
                    containsPrefix = true;
                }
                else
                {
                    Logger.LogWarning($"{character.Data.Name} found no corporation info for corporation {character.Data.CorporationId}");
                }
                

                if (containsPrefix)
                {
                    retVal.Append(" ");
                }

                retVal.Append(character.Data.Name);

            }
            else
            {
                Logger.LogError($"{characterID} could not get character info for name {character.ResponseCode}");
            }
            return retVal.ToString();
        }
    }
}
