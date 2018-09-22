using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem.Corporation
{
    [ESIItem("CorporationTitle", "/corporations/{corporation_id}/titles/")]
    public class CorporationTitle : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "grantable_roles")]
        public IEnumerable<eESIRole> GrantableRoles { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "grantable_roles_at_base")]
        public IEnumerable<eESIRole> GrantableRolesAtBase { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "grantable_roles_at_hq")]
        public IEnumerable<eESIRole> GrantableRolesAtHq { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "grantable_roles_at_other")]
        public IEnumerable<eESIRole> GrantableRolesAtOther { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public String Name { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "roles")]
        public IEnumerable<eESIRole> Roles { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "roles_at_base")]
        public IEnumerable<eESIRole> RolesAtBase { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "roles_at_hq")]
        public IEnumerable<eESIRole> RolesAtHq { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "roles_at_other")]
        public IEnumerable<eESIRole> RolesAtOther { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "title_id")]
        public Int32 TitleId { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CorporationTitle", "Save");
            command.AddParameter("GrantableRoles", System.Data.DbType.Int64).Value = GrantableRoles.ToFlag();
            command.AddParameter("GrantableRolesAtBase", System.Data.DbType.Int64).Value = GrantableRolesAtBase.ToFlag();
            command.AddParameter("GrantableRolesAtHq", System.Data.DbType.Int64).Value = GrantableRolesAtHq.ToFlag();
            command.AddParameter("GrantableRolesAtOther", System.Data.DbType.Int64).Value = GrantableRolesAtOther.ToFlag();
            command.AddParameter("Name", System.Data.DbType.String).Value = Name;
            command.AddParameter("Roles", System.Data.DbType.Int64).Value = Roles.ToFlag();
            command.AddParameter("RolesAtBase", System.Data.DbType.Int64).Value = RolesAtBase.ToFlag();
            command.AddParameter("RolesAtHq", System.Data.DbType.Int64).Value = RolesAtHq.ToFlag();
            command.AddParameter("RolesAtOther", System.Data.DbType.Int64).Value = RolesAtOther.ToFlag();
            command.AddParameter("TitleId", System.Data.DbType.Int32).Value = TitleId;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            GrantableRoles = adapter.GetValue("GrantableRoles", 0L).ToList<eESIRole>(); 
            GrantableRolesAtBase = adapter.GetValue("GrantableRolesAtBase", 0L).ToList<eESIRole>();
            GrantableRolesAtHq = adapter.GetValue("GrantableRolesAtHq", 0L).ToList<eESIRole>();
            GrantableRolesAtOther = adapter.GetValue("GrantableRolesAtOther", 0L).ToList<eESIRole>();
            Name = adapter.GetValue("Name", string.Empty);
            Roles = adapter.GetValue("Roles", 0L).ToList<eESIRole>();
            RolesAtBase = adapter.GetValue("RolesAtBase", 0L).ToList<eESIRole>();
            RolesAtHq = adapter.GetValue("RolesAtHq", 0L).ToList<eESIRole>();
            RolesAtOther = adapter.GetValue("RolesAtOther", 0L).ToList<eESIRole>();
            TitleId = adapter.GetValue("TitleId", 0);
        }

        public static Task<ESICollectionCallResponse<CorporationTitle>> GetCorporationTitles(IESIAuthenticationClient client, ICommandController controller, long corporationID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask,bool oldData = false)
        {
            return GetCollection<CorporationTitle>(client, controller, new Dictionary<string, object>() { { "corporation_id", corporationID } }, authenticationTokenTask, oldData);
        }

        public static CorporationTitle ForTitleID(ICommandController controller,int titleID)
        {
            DataCommand command = new DataCommand("CorporationTitle", "ForTitleID");
            command.AddParameter("TitleId", System.Data.DbType.Int32).Value = titleID;
            return LoadSingle<CorporationTitle>(controller.ExecuteCollectionCommand(command));
        }
    }
}
