using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using Eve.ESI.Standard.Authentication.Client;

namespace Eve.ESI.Standard.DataItem.Character
{
    [ESIItem("CharacterRoles", "/characters/{character_id}/roles/")]
    public class CharacterRoles : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "roles")]
        public IEnumerable<eESIRole> Roles { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "roles_at_base")]
        public IEnumerable<eESIRole> RolesAtBase { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "roles_at_hq")]
        public IEnumerable<eESIRole> RolesAtHq { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "roles_at_other")]
        public IEnumerable<eESIRole> RolesAtOther { get; private set; }

        public eESIRole RolesEnum
        {
            get
            {
                return (eESIRole)Roles.ToFlag();
            }
        }

        public eESIRole RolesAtBaseEnum
        {
            get
            {
                return (eESIRole)RolesAtBase.ToFlag();
            }
        }
        public eESIRole RolesAtHqEnum
        {
            get
            {
                return (eESIRole)RolesAtHq.ToFlag();
            }
        }
        public eESIRole RolesAtOtherEnum
        {
            get
            {
                return (eESIRole)RolesAtOther.ToFlag();
            }
        }
        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterRoles", "Save");
            command.AddParameter("Roles", System.Data.DbType.Int64).Value = Roles.ToFlag();
            command.AddParameter("RolesAtBase", System.Data.DbType.Int64).Value = RolesAtBase.ToFlag();
            command.AddParameter("RolesAtHq", System.Data.DbType.Int64).Value = RolesAtHq.ToFlag();
            command.AddParameter("RolesAtOther", System.Data.DbType.Int64).Value = RolesAtOther.ToFlag();
            return command;
        }
        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            Roles = adapter.GetValue("Roles", 0L).ToList<eESIRole>();
            RolesAtBase = adapter.GetValue("RolesAtBase", 0L).ToList<eESIRole>();
            RolesAtHq = adapter.GetValue("RolesAtHq", 0L).ToList<eESIRole>();
            RolesAtOther = adapter.GetValue("RolesAtOther", 0L).ToList<eESIRole>();
        }
        public long RoleFlag()
        {
            return Roles.ToFlag();
        }
        public bool HasRole(eESIRole role)
        {
            return Roles.HasFlag(role);
        }
        public bool HasRoleAtBase(eESIRole role)
        {
            return RolesAtBase.HasFlag(role);
        }
        public bool HasRoleAtHq(eESIRole role)
        {
            return RolesAtHq.HasFlag(role);
        }
        public bool HasRoleAtOther(eESIRole role)
        {
            return RolesAtOther.HasFlag(role);
        }

        public static System.Threading.Tasks.Task<ESICallResponse<CharacterRoles>> GetCharacterRoles(IESIAuthenticationClient client, ICommandController controller, long characterID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask,bool olddata = false)
        {
            return GetItem<CharacterRoles>(client, controller, new Dictionary<string, object>() { { "character_id", characterID } }, authenticationTokenTask: authenticationTokenTask, alwaysReturnOldData: olddata);
        }
    }
}
