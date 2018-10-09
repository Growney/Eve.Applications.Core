using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.DataItem.Alliance;
using Eve.ESI.Standard.DataItem.Character;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.DataItem.Search;
using Eve.Static.Standard;
using Eve.Static.Standard.chr;
using Gware.Standard.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.GUI.Web.Models.Search
{
    public class SearchResult
    {
        public LazyLoadedList<long, ESICallResponse<CharacterInfo>> Characters { get; }
        public LazyLoadedList<long, ESICallResponse<AllianceInfo>> Alliances { get; }
        public LazyLoadedList<long, ESICallResponse<CorporationInfo>> Corporations { get; }
        public LazyLoadedList<long,ChrFactions> Factions { get; }

        public string ResultID { get; }

        public SearchResult(IStaticDataCache cache,IPublicDataProvider dataProvider, SearchResults results,bool oldData, string resultID)
        {
            ResultID = resultID;
            Characters = new LazyLoadedList<long, ESICallResponse<CharacterInfo>>(x => dataProvider.GetCharacterInfo(x, oldData), results.Character);
            Alliances = new LazyLoadedList<long, ESICallResponse<AllianceInfo>>(x => dataProvider.GetAllianceInfo((int)x, oldData), results.Alliance);
            Corporations = new LazyLoadedList<long, ESICallResponse<CorporationInfo>>(x => dataProvider.GetCorporationInfo((int)x, oldData), results.Corporation);
            Factions = new LazyLoadedList<long, ChrFactions>(x => cache.GetItemAsync<ChrFactions>((int)x),results.Faction);
        }

    }
}
