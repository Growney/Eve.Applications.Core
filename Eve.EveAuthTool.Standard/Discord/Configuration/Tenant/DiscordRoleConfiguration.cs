using System;
using System.Collections.Generic;
using System.Text;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.EveAuthTool.Standard.Discord.Configuration.Tenant
{
    public class DiscordRoleConfiguration : Gware.Standard.Storage.StoredObjectBase
    {
        public override long Id { get; set; }
        public string Name { get; set; }
        public string GuildInviteID { get; set; }
        public HashSet<ulong> AssignedRoles { get; } = new HashSet<ulong>();
      

        protected override void OnLoad(IDataAdapter adapter)
        {
            Name = adapter.GetValue("Name", string.Empty);
            GuildInviteID = adapter.GetValue("GuildInviteID", string.Empty);

            AssignedRoles.Clear();
            List<byte[]> values = Load<LoadedFromAdapterBytes>(adapter.Controller.ExecuteCollectionCommand(GetRoleSelectCommand())).ToList();
            foreach(byte[] value in values)
            {
                AssignedRoles.Add(BitConverter.ToUInt64(value,0));
            }
        }
        protected override void AddParametersToSave(IDataCommand command)
        {
            command.AddParameter("Name", System.Data.DbType.String).Value = Name;
            command.AddParameter("GuildInviteID", System.Data.DbType.String).Value = GuildInviteID;

            List<byte[]> values = new List<byte[]>();
            foreach(ulong roleID in AssignedRoles)
            {
                values.Add(BitConverter.GetBytes(roleID));
            }
            command.AddParameter("RoleIDs", "DataList").Value = values.CreateTableData("DataValue");

        }

        public void AddRoles(IEnumerable<ulong> values)
        {
            foreach(ulong value in values)
            {
                if (!AssignedRoles.Contains(value))
                {
                    AssignedRoles.Add(value);
                }
            }
        }

        private IDataCommand GetRoleSelectCommand()
        {
            DataCommand command = new DataCommand("DiscordRoleConfiguration", "RolesForConfiguration");
            command.AddParameter("Id", System.Data.DbType.Int64).Value = Id;
            return command;
        }

        public static List<DiscordRoleConfiguration> All(ICommandController controller)
        {
            DataCommand command = new DataCommand("DiscordRoleConfiguration", "All");
            return Load<DiscordRoleConfiguration>(controller.ExecuteCollectionCommand(command));
        }
    

    }
}
