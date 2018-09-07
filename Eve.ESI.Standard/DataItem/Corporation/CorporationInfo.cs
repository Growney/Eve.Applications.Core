using System;
using System.Collections.Generic;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem.Corporation
{
    [ESIItem("CorporationInfo", "/corporations/{corporation_id}/")]
    public class CorporationInfo : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "alliance_id")]
        public Int32 AllianceId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ceo_id")]
        public Int32 CeoId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "creator_id")]
        public Int32 CreatorId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "date_founded")]
        public DateTime DateFounded { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public String Description { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "faction_id")]
        public Int32 FactionId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "home_station_id")]
        public Int32 HomeStationId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "member_count")]
        public Int32 MemberCount { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "shares")]
        public Int64 Shares { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "tax_rate")]
        public Double TaxRate { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ticker")]
        public String Ticker { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "url")]
        public String Url { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CorporationInfo", "Save");
            command.AddParameter("AllianceId", System.Data.DbType.Int32).Value = AllianceId;
            command.AddParameter("CeoId", System.Data.DbType.Int32).Value = CeoId;
            command.AddParameter("CreatorId", System.Data.DbType.Int32).Value = CreatorId;
            command.AddParameter("DateFounded", System.Data.DbType.DateTime).Value = DateFounded;
            command.AddParameter("Description", System.Data.DbType.String).Value = Description;
            command.AddParameter("FactionId", System.Data.DbType.Int32).Value = FactionId;
            command.AddParameter("HomeStationId", System.Data.DbType.Int32).Value = HomeStationId;
            command.AddParameter("MemberCount", System.Data.DbType.Int32).Value = MemberCount;
            command.AddParameter("Name", System.Data.DbType.String).Value = Name;
            command.AddParameter("Shares", System.Data.DbType.Int64).Value = Shares;
            command.AddParameter("TaxRate", System.Data.DbType.Double).Value = TaxRate;
            command.AddParameter("Ticker", System.Data.DbType.String).Value = Ticker;
            command.AddParameter("Url", System.Data.DbType.String).Value = Url;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            AllianceId = adapter.GetValue("AllianceId", 0);
            CeoId = adapter.GetValue("CeoId", 0);
            CreatorId = adapter.GetValue("CreatorId", 0);
            DateFounded = adapter.GetValue("DateFounded", DateTime.MinValue);
            Description = adapter.GetValue("Description", string.Empty);
            FactionId = adapter.GetValue("FactionId", 0);
            HomeStationId = adapter.GetValue("HomeStationId", 0);
            MemberCount = adapter.GetValue("MemberCount", 0);
            Name = adapter.GetValue("Name", string.Empty);
            Shares = adapter.GetValue("Shares", 0L);
            TaxRate = adapter.GetValue("TaxRate", 0.0d);
            Ticker = adapter.GetValue("Ticker", string.Empty);
            Url = adapter.GetValue("Url", string.Empty);
        }

        public static System.Threading.Tasks.Task<ESICallResponse<CorporationInfo>> GetCorporationInfo(IESIAuthenticationClient authClient,ICommandController controller,int corporationID,bool alwaysReturnOldData = false)
        {
            return GetItem<CorporationInfo>(authClient,controller,new Dictionary<string, object>() { { "corporation_id", corporationID } },alwaysReturnOldData:alwaysReturnOldData);
        }
    }
}
