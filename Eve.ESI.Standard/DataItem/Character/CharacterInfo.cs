using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;

namespace Eve.ESI.Standard.DataItem.Character
{
    [ESIItem("CharacterInfo", "/characters/{character_id}/")]
    public class CharacterInfo : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "alliance_id")]
        public Int32 AllianceId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ancestry_id")]
        public Int32 AncestryId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "birthday")]
        public DateTime Birthday { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "bloodline_id")]
        public Int32 BloodlineId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "corporation_id")]
        public Int32 CorporationId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public String Description { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "faction_id")]
        public Int32 FactionId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "gender")]
        public String Gender { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "race_id")]
        public Int32 RaceId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "security_status")]
        public Double SecurityStatus { get; private set; }

        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterInfo", "Save");
            command.AddParameter("AllianceId", System.Data.DbType.Int32).Value = AllianceId;
            command.AddParameter("AncestryId", System.Data.DbType.Int32).Value = AncestryId;
            command.AddParameter("Birthday", System.Data.DbType.DateTime).Value = Birthday;
            command.AddParameter("BloodlineId", System.Data.DbType.Int32).Value = BloodlineId;
            command.AddParameter("CorporationId", System.Data.DbType.Int32).Value = CorporationId;
            command.AddParameter("Description", System.Data.DbType.String).Value = Description;
            command.AddParameter("FactionId", System.Data.DbType.Int32).Value = FactionId;
            command.AddParameter("Gender", System.Data.DbType.String).Value = Gender;
            command.AddParameter("Name", System.Data.DbType.String).Value = Name;
            command.AddParameter("RaceId", System.Data.DbType.Int32).Value = RaceId;
            command.AddParameter("SecurityStatus", System.Data.DbType.Double).Value = SecurityStatus;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            AllianceId = adapter.GetValue("AllianceId", 0);
            AncestryId = adapter.GetValue("AncestryId", 0);
            Birthday = adapter.GetValue("Birthday", DateTime.MinValue);
            BloodlineId = adapter.GetValue("BloodlineId", 0);
            CorporationId = adapter.GetValue("CorporationId", 0);
            Description = adapter.GetValue("Description", string.Empty);
            FactionId = adapter.GetValue("FactionId", 0);
            Gender = adapter.GetValue("Gender", string.Empty);
            Name = adapter.GetValue("Name", string.Empty);
            RaceId = adapter.GetValue("RaceId", 0);
            SecurityStatus = adapter.GetValue("SecurityStatus", 0.0d);
        }

        public static System.Threading.Tasks.Task<ESICallResponse<CharacterInfo>> GetCharacterInfo(IESIAuthenticationClient client,ICommandController controller,long characterID,bool alwaysReturnOldData = false)
        {
            return GetItem<CharacterInfo>(client,controller,new Dictionary<string, object>() { { "character_id", characterID } }, alwaysReturnOldData: alwaysReturnOldData);
        }
    }
}
