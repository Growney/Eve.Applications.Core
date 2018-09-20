using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Search
{
    [ESIItem("ESIItemParent", "/search/", typeof(IDIntegerCollectionItem))]
    public class SearchResults : ESIParentItemBase
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "agent")]
        public IEnumerable<long> Agent { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "alliance")]
        public IEnumerable<long> Alliance { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "character")]
        public IEnumerable<long> Character { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "constellation")]
        public IEnumerable<long> Constellation { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "corporation")]
        public IEnumerable<long> Corporation { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "faction")]
        public IEnumerable<long> Faction { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "inventory_type")]
        public IEnumerable<long> InventoryType { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "region")]
        public IEnumerable<long> Region { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "solar_system")]
        public IEnumerable<long> SolarSystem { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "station")]
        public IEnumerable<long> Station { get; private set; }

        protected override void LoadChildren(ICommandController controller)
        {
            Agent = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.agent, CallID);
            Alliance = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.alliance, CallID);
            Character = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.character, CallID);
            Constellation = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.constellation, CallID);
            Corporation = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.corporation, CallID);
            Faction = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.faction, CallID);
            InventoryType = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.inventory_type, CallID);
            Region = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.region, CallID);
            SolarSystem = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.solar_system, CallID);
            Station = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, (int)eSearchEntity.station, CallID);
        }
        protected override void SaveChildren(ICommandController controller)
        {
            Agent.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.agent, CallID).Save(controller);
            Alliance.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.alliance, CallID).Save(controller);
            Character.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.character, CallID).Save(controller);
            Constellation.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.constellation, CallID).Save(controller);
            Corporation.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.corporation, CallID).Save(controller);
            Faction.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.faction, CallID).Save(controller);
            InventoryType.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.inventory_type, CallID).Save(controller);
            Region.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.region, CallID).Save(controller);
            SolarSystem.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.solar_system, CallID).Save(controller);
            Station.ToIDIntegerCollection<IDIntegerCollectionItem>((int)eSearchEntity.station, CallID).Save(controller);
        }

        public static Task<ESICallResponse<SearchResults>> Search(IESIAuthenticationClient client, ICommandController controller, string query, eSearchEntity entities, bool strict = false)
        {
            return GetItem<SearchResults>(client, controller, queryParameters: new Dictionary<string, string>() { { "search", query }, { "categories", entities.ToDelimitedString<eSearchEntity>() }, { "strict", strict.ToString() } });
        }

        public static eESIEntityType SearchTypeToEntityType(eSearchEntity type)
        {
            switch (type)
            {
                case eSearchEntity.alliance:
                    return eESIEntityType.alliance;
                case eSearchEntity.character:
                    return eESIEntityType.character;
                case eSearchEntity.corporation:
                    return eESIEntityType.corporation;
                case eSearchEntity.faction:
                    return eESIEntityType.faction;
                default:
                    throw new ArgumentException("Not an entity");
            }
        }
    }
}
