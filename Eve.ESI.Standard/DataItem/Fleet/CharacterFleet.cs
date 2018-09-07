using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem.Fleet
{
    [ESIItem("CharacterFleet", "/characters/{character_id}/fleet/")]
    public class CharacterFleet : ESIItemBase
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "fleet_id")]
        public Int64 FleetId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "role")]
        public eFleetRole Role { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "squad_id")]
        public Int64 SquadId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "wing_id")]
        public Int64 WingId { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterFleet", "Save");
            command.AddParameter("FleetId", System.Data.DbType.Int64).Value = FleetId;
            command.AddParameter("Role", System.Data.DbType.Int32).Value = (int)Role;
            command.AddParameter("SquadId", System.Data.DbType.Int64).Value = SquadId;
            command.AddParameter("WingId", System.Data.DbType.Int64).Value = WingId;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            FleetId = adapter.GetValue("FleetId", 0L);
            Role = (eFleetRole)adapter.GetValue("Role", 0);
            SquadId = adapter.GetValue("SquadId", 0L);
            WingId = adapter.GetValue("WingId", 0L);
        }

        public static async Task<ESICallResponse<CharacterFleet>> GetCharacterFleet(IESIAuthenticationClient client, ICommandController controller, long characterID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask, bool oldData = false)
        {
            Dictionary<string, object> values  = new Dictionary<string, object>() { { "character_id", characterID } };
            ESICallResponse<CharacterFleet> retVal = await GetItem<CharacterFleet>(client, controller, values, authenticationTokenTask: authenticationTokenTask, alwaysReturnOldData: oldData);

            if (retVal.HasData && retVal.ResponseCode == System.Net.HttpStatusCode.NotFound)
            {
                ClearStoredData<CharacterFleet>(controller, values);
            }

            return retVal;
        }

        
    }
}
