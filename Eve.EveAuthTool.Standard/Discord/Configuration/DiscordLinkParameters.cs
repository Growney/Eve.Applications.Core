using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public class DiscordLinkParameter : Gware.Standard.Storage.StoredObjectBase
    {
        public Guid LinkGuid { get; set; }
        public long TenantID { get; private set; }
        public override long Id { get; set; }
        
        public DiscordLinkParameter()
        {

        }

        public DiscordLinkParameter(long tenantID)
        {
            TenantID = tenantID;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            LinkGuid = new Guid(adapter.GetValue("LinkGuid", string.Empty));
            TenantID = adapter.GetValue("TenantID", 0L);
        }

        protected override void AddParametersToSave(IDataCommand command)
        {
            command.AddParameter("LinkGuid", System.Data.DbType.Guid).Value = LinkGuid;
            command.AddParameter("TenantID", System.Data.DbType.Int64).Value = TenantID;
        }

        public static DiscordLinkParameter Recall(ICommandController controller,Guid guid)
        {
            DataCommand command = new DataCommand("DiscordLinkParameter", "Recall");
            command.AddParameter("LinkGuid", System.Data.DbType.Guid).Value = guid;
            return LoadSingle<DiscordLinkParameter>(controller.ExecuteCollectionCommand(command));
        }

        public static void Discard(ICommandController controller,Guid guid)
        {
            DataCommand command = new DataCommand("DiscordLinkParameter", "Discard");
            command.AddParameter("LinkGuid", System.Data.DbType.Guid).Value = guid;
            controller.ExecuteQuery(command);
        }
        
    }
}
