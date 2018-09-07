using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Fleet
{

    [ESIItem("FleetMember", "/fleets/{fleet_id}/members/")]
    public class FleetMember : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "character_id")]
        public Int32 CharacterId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "join_time")]
        public DateTime JoinTime { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "role")]
        public eFleetRole Role { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "role_name")]
        public String RoleName { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ship_type_id")]
        public Int32 ShipTypeId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "solar_system_id")]
        public Int32 SolarSystemId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "squad_id")]
        public Int64 SquadId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "station_id")]
        public Int64 StationId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "takes_fleet_warp")]
        public Boolean TakesFleetWarp { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "wing_id")]
        public Int64 WingId { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("FleetMember", "Save");
            command.AddParameter("CharacterId", System.Data.DbType.Int32).Value = CharacterId;
            command.AddParameter("JoinTime", System.Data.DbType.DateTime).Value = JoinTime;
            command.AddParameter("Role", System.Data.DbType.Int32).Value = (int)Role;
            command.AddParameter("RoleName", System.Data.DbType.String).Value = RoleName;
            command.AddParameter("ShipTypeId", System.Data.DbType.Int32).Value = ShipTypeId;
            command.AddParameter("SolarSystemId", System.Data.DbType.Int32).Value = SolarSystemId;
            command.AddParameter("SquadId", System.Data.DbType.Int64).Value = SquadId;
            command.AddParameter("StationId", System.Data.DbType.Int64).Value = StationId;
            command.AddParameter("TakesFleetWarp", System.Data.DbType.Boolean).Value = TakesFleetWarp;
            command.AddParameter("WingId", System.Data.DbType.Int64).Value = WingId;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            CharacterId = adapter.GetValue("CharacterId", 0);
            JoinTime = adapter.GetValue("JoinTime", DateTime.MinValue);
            Role = (eFleetRole)adapter.GetValue("Role", 0);
            RoleName = adapter.GetValue("RoleName", string.Empty);
            ShipTypeId = adapter.GetValue("ShipTypeId", 0);
            SolarSystemId = adapter.GetValue("SolarSystemId", 0);
            SquadId = adapter.GetValue("SquadId", 0L);
            StationId = adapter.GetValue("StationId", 0L);
            TakesFleetWarp = adapter.GetValue("TakesFleetWarp", false);
            WingId = adapter.GetValue("WingId", 0L);
        }

        public static async Task<ESICollectionCallResponse<FleetMember>> GetFleetMembers(IESIAuthenticationClient client, ICommandController controller, long fleetID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask, bool oldData = false)
        {
            Dictionary<string, object> values = new Dictionary<string, object>() { { "fleet_id", fleetID } };
            ESICollectionCallResponse<FleetMember> retVal = await GetCollection<FleetMember>(client, controller, values, authenticationTokenTask, oldData);

            if (retVal.Data != null && retVal.ResponseCode == System.Net.HttpStatusCode.NotFound)
            {
                ClearStoredData<FleetMember>(controller, values);
            }
            return retVal;
        }
    }
}
