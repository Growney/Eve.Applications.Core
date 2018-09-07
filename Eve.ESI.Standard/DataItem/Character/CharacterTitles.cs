using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using System;

namespace Eve.ESI.Standard.DataItem
{
    [ESIItem("CharacterTitles", "/characters/{character_id}/titles/")]
    public class CharacterTitles : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "title_id")]
        public Int32 TitleId { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterTitles", "Save");
            command.AddParameter("Name", System.Data.DbType.String).Value = Name;
            command.AddParameter("TitleId", System.Data.DbType.Int32).Value = TitleId;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            Name = adapter.GetValue("Name", string.Empty);
            TitleId = adapter.GetValue("TitleId", 0);
        }
    }
}
