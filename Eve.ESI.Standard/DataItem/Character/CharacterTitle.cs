using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Character
{
    [ESIItem("CharacterTitle", "/characters/{character_id}/titles/")]
    public class CharacterTitle : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "title_id")]
        public Int32 TitleId { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterTitle", "Save");
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

        public static System.Threading.Tasks.Task<ESICollectionCallResponse<CharacterTitle>> GetCharacterTitles(IESIAuthenticationClient client, ICommandController controller, long characterID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask, bool olddata = false)
        {
            return GetCollection<CharacterTitle>(client, controller, new Dictionary<string, object>() { { "character_id", characterID } }, authenticationTokenTask: authenticationTokenTask, alwaysReturnOldData: olddata);
        }
    }
}
