using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;

namespace Eve.ESI.Standard.DataItem.Alliance
{
    [ESIItem("AllianceInfo", "/alliances/{alliance_id}/")]
    public class AllianceInfo : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "creator_corporation_id")]
        public Int32 CreatorCorporationId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "creator_id")]
        public Int32 CreatorId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "date_founded")]
        public DateTime DateFounded { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "executor_corporation_id")]
        public Int32 ExecutorCorporationId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "faction_id")]
        public Int32 FactionId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ticker")]
        public String Ticker { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("AllianceInfo", "Save");
            command.AddParameter("CreatorCorporationId", System.Data.DbType.Int32).Value = CreatorCorporationId;
            command.AddParameter("CreatorId", System.Data.DbType.Int32).Value = CreatorId;
            command.AddParameter("DateFounded", System.Data.DbType.DateTime).Value = DateFounded;
            command.AddParameter("ExecutorCorporationId", System.Data.DbType.Int32).Value = ExecutorCorporationId;
            command.AddParameter("FactionId", System.Data.DbType.Int32).Value = FactionId;
            command.AddParameter("Name", System.Data.DbType.String).Value = Name;
            command.AddParameter("Ticker", System.Data.DbType.String).Value = Ticker;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            CreatorCorporationId = adapter.GetValue("CreatorCorporationId", 0);
            CreatorId = adapter.GetValue("CreatorId", 0);
            DateFounded = adapter.GetValue("DateFounded", DateTime.MinValue);
            ExecutorCorporationId = adapter.GetValue("ExecutorCorporationId", 0);
            FactionId = adapter.GetValue("FactionId", 0);
            Name = adapter.GetValue("Name", string.Empty);
            Ticker = adapter.GetValue("Ticker", string.Empty);
        }

        public static System.Threading.Tasks.Task<ESICallResponse<AllianceInfo>> GetAllianceInfo(IESIAuthenticationClient client,ICommandController controller,int allianceID,bool alwaysReturnOldData = false)
        {
            return GetItem<AllianceInfo>(client,controller,new Dictionary<string, object>() { { "alliance_id", allianceID } }, alwaysReturnOldData: alwaysReturnOldData);
        }
    }
}
