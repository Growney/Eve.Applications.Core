using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem.Skills
{
    [ESIItem("CharacterAttributes", "/characters/{character_id}/attributes/")]
    public class CharacterAttributes : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "accrued_remap_cooldown_date")]
        public DateTime AccruedRemapCooldownDate { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "bonus_remaps")]
        public Int32 BonusRemaps { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "charisma")]
        public Int32 Charisma { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "intelligence")]
        public Int32 Intelligence { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "last_remap_date")]
        public DateTime LastRemapDate { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "memory")]
        public Int32 Memory { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "perception")]
        public Int32 Perception { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "willpower")]
        public Int32 Willpower { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterAttributes", "Save");
            command.AddParameter("AccruedRemapCooldownDate", System.Data.DbType.DateTime).Value = AccruedRemapCooldownDate;
            command.AddParameter("BonusRemaps", System.Data.DbType.Int32).Value = BonusRemaps;
            command.AddParameter("Charisma", System.Data.DbType.Int32).Value = Charisma;
            command.AddParameter("Intelligence", System.Data.DbType.Int32).Value = Intelligence;
            command.AddParameter("LastRemapDate", System.Data.DbType.DateTime).Value = LastRemapDate;
            command.AddParameter("Memory", System.Data.DbType.Int32).Value = Memory;
            command.AddParameter("Perception", System.Data.DbType.Int32).Value = Perception;
            command.AddParameter("Willpower", System.Data.DbType.Int32).Value = Willpower;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            AccruedRemapCooldownDate = adapter.GetValue("AccruedRemapCooldownDate", DateTime.MinValue);
            BonusRemaps = adapter.GetValue("BonusRemaps", 0);
            Charisma = adapter.GetValue("Charisma", 0);
            Intelligence = adapter.GetValue("Intelligence", 0);
            LastRemapDate = adapter.GetValue("LastRemapDate", DateTime.MinValue);
            Memory = adapter.GetValue("Memory", 0);
            Perception = adapter.GetValue("Perception", 0);
            Willpower = adapter.GetValue("Willpower", 0);
        }

        public static System.Threading.Tasks.Task<ESICallResponse<CharacterAttributes>> GetAttributes(IESIAuthenticationClient client,ICommandController controller,long characterID,Func<Task<ESITokenRefreshResponse>> authenticationTokenTask)
        {
            return GetItem<CharacterAttributes>(client,controller,new Dictionary<string, object>() { { "character_id", characterID } }, authenticationTokenTask: authenticationTokenTask);
        }
    }
}
