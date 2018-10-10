using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.Static.Standard;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.EveAuthTool.Standard.Security.Rules
{
    public class Role : StoredObjectBase
    {
        public static readonly Role None = new Role();
        public override long Id { get; set; }
        public string Name { get; set; }
        public int Ordinal { get; set; }
        public eRulePermission Permissions { get; set; }
        public long DiscordRoleConfigurationID { get; set; }

        public Role()
        {
            Permissions = eRulePermission.None;
        }

        protected override void AddParametersToSave(IDataCommand command)
        {
            command.AddParameter("Name", System.Data.DbType.String).Value = Name ?? string.Empty;
            command.AddParameter("Permissions", System.Data.DbType.Int64).Value = (long)Permissions;
            command.AddParameter("Ordinal", System.Data.DbType.Int32).Value = Ordinal;
            command.AddParameter("DiscordRoleConfigurationID", System.Data.DbType.Int64).Value = DiscordRoleConfigurationID;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            Name = adapter.GetValue("Name", string.Empty);
            Permissions = (eRulePermission)adapter.GetValue("Permissions", 0L);
            Ordinal = adapter.GetValue("Ordinal", 0);
            DiscordRoleConfigurationID = adapter.GetValue("DiscordRoleConfigurationID", 0L);
        }

        public static List<Role> All(ICommandController controller)
        {
            DataCommand command = new DataCommand("Role", "All");
            return Load<Role>(controller.ExecuteCollectionCommand(command));
        }

        public static async Task<Role> ForAccount(IESIAuthenticatedConfig esiConfiguration,ICommandController tenantController,IStaticDataCache cache,IPublicDataProvider publicDataProvider,UserAccount user)
        {
            Role retVal = null;
            List<AuthenticatedEntity> characters = AuthenticatedEntity.GetForAccount(esiConfiguration, tenantController, cache, publicDataProvider, user);
            foreach (AuthenticatedEntity character in characters)
            {
                retVal = await AuthRule.GetEntityRole(esiConfiguration, tenantController, cache, publicDataProvider, character);
                if (retVal != null)
                {
                    break;
                }
            }
            return retVal;
        }
    }
}
