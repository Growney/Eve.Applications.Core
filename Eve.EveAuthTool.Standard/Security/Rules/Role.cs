using System.Collections.Generic;
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

        public Role()
        {
            Permissions = eRulePermission.None;
        }

        protected override void AddParametersToSave(IDataCommand command)
        {
            command.AddParameter("Name", System.Data.DbType.String).Value = Name ?? string.Empty;
            command.AddParameter("Permissions", System.Data.DbType.Int64).Value = (long)Permissions;
            command.AddParameter("Ordinal", System.Data.DbType.Int32).Value = Ordinal;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            Name = adapter.GetValue("Name", string.Empty);
            Permissions = (eRulePermission)adapter.GetValue("Permissions", 0L);
            Ordinal = adapter.GetValue("Ordinal", 0);
        }

        public static List<Role> All(ICommandController controller)
        {
            DataCommand command = new DataCommand("Role", "All");
            return Load<Role>(controller.ExecuteCollectionCommand(command));
        }
    }
}
