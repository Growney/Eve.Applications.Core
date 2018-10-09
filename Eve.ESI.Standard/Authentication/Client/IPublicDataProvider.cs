using Eve.ESI.Standard.DataItem.Alliance;
using Eve.ESI.Standard.DataItem.Character;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.DataItem.Search;
using Eve.Static.Standard;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.Authentication.Client
{
    public interface IPublicDataProvider
    {
        Task<ESICallResponse<CharacterInfo>> GetCharacterInfo(long characterID, bool oldData = false);
        Task<ESICallResponse<AllianceInfo>> GetAllianceInfo(int allianceID, bool oldData = false);
        Task<ESICallResponse<CorporationInfo>> GetCorporationInfo(int corporationID, bool oldData = false);
        Task<ESICallResponse<SearchResults>> Search(string query, eSearchEntity entities);
        Task<string> GetEntityName(eESIEntityType entityType, long entityID);
        Task<string> GetTaggedCharacterName(long characterID);
    }
}
