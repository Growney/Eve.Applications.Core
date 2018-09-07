using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eve.ESI.Standard.Relationships
{
    [DebuggerDisplay("{FromEntityID} - {FromEntityType} to {ToEntityID} - {ToEntityType} {Relationship}")]
    public class EntityRelationship : StoredObjectBase
    {
        public long FromEntityID { get; private set; }
        public eESIEntityType FromEntityType { get; private set; }
        public long ToEntityID { get; private set; }
        public eESIEntityType ToEntityType { get; private set; }
        public eESIEntityRelationship Relationship { get; private set; }
        public DateTime CheckedOn { get; private set; }
        public override long Id { get; set; }

        public EntityRelationship()
        {

        }

        public EntityRelationship(long fromEntityID, eESIEntityType fromEntityType,long toEntityID,eESIEntityType toEntityType,eESIEntityRelationship relationship)
        {
            FromEntityID = fromEntityID;
            FromEntityType = fromEntityType;
            ToEntityID = toEntityID;
            ToEntityType = toEntityType;
            Relationship = relationship;
            CheckedOn = DateTime.UtcNow;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            FromEntityID = adapter.GetValue("FromEntityID", 0L);
            FromEntityType = (eESIEntityType)adapter.GetValue("FromEntityType", 0);
            ToEntityID = adapter.GetValue("ToEntityID", 0L);
            ToEntityType = (eESIEntityType)adapter.GetValue("ToEntityType", 0);
            Relationship = (eESIEntityRelationship)adapter.GetValue("Relationship", 0);
            CheckedOn = adapter.GetValue("CheckedOn", DateTime.MinValue);
        }

        public override IDataCommand CreateSaveCommand()
        {
            DataCommand command = new DataCommand("EntityRelationship", "Save");

            command.AddParameter("Id", System.Data.DbType.Int64).Value = Id;
            command.AddParameter("FromEntityID", System.Data.DbType.Int64).Value = FromEntityID;
            command.AddParameter("FromEntityType", System.Data.DbType.Int32).Value = (int)FromEntityType;
            command.AddParameter("ToEntityID", System.Data.DbType.Int64).Value = ToEntityID;
            command.AddParameter("ToEntityType", System.Data.DbType.Int32).Value = (int)ToEntityType;
            command.AddParameter("Relationship", System.Data.DbType.Int32).Value = (int)Relationship;
            command.AddParameter("CheckedOn", System.Data.DbType.DateTime).Value = CheckedOn;

            return command;

        }

        public override IDataCommand CreateDeleteCommand()
        {
            throw new NotImplementedException();
        }

        public override IDataCommand CreateLoadFromPrimaryKey(long primaryKey)
        {
            throw new NotImplementedException();
        }

        public static void RemoveForFrom(ICommandController controller, long fromEntityID, eESIEntityType fromEntityType)
        {
            DataCommand command = new DataCommand("EntityRelationship", "RemoveForFrom");
            command.AddParameter("FromEntityID", System.Data.DbType.Int64).Value = fromEntityID;
            command.AddParameter("FromEntityType", System.Data.DbType.Int32).Value = (int)fromEntityType;
            controller.ExecuteQuery(command);
        }
        public static void RemoveForFromTo(ICommandController controller, long fromEntityID, eESIEntityType fromEntityType,long toEntityID,eESIEntityType toEntityType)
        {
            DataCommand command = new DataCommand("EntityRelationship", "RemoveForFromTo");
            command.AddParameter("FromEntityID", System.Data.DbType.Int64).Value = fromEntityID;
            command.AddParameter("FromEntityType", System.Data.DbType.Int32).Value = (int)fromEntityType;
            command.AddParameter("ToEntityID", System.Data.DbType.Int64).Value = toEntityID;
            command.AddParameter("ToEntityType", System.Data.DbType.Int32).Value = (int)toEntityType;
            controller.ExecuteQuery(command);
        }
        public static List<EntityRelationship> SelectForFrom(ICommandController controller, long fromEntityID, eESIEntityType fromEntityType)
        {
            DataCommand command = new DataCommand("EntityRelationship", "SelectForFrom");
            command.AddParameter("FromEntityID", System.Data.DbType.Int64).Value = fromEntityID;
            command.AddParameter("FromEntityType", System.Data.DbType.Int32).Value = (int)fromEntityType;
            return Load<EntityRelationship>(controller.ExecuteCollectionCommand(command));
        }
        public static List<EntityRelationship> SelectForTo(ICommandController controller, long toEntityID, eESIEntityType toEntityType)
        {
            DataCommand command = new DataCommand("EntityRelationship", "SelectForTo");
            command.AddParameter("ToEntityID", System.Data.DbType.Int64).Value = toEntityID;
            command.AddParameter("ToEntityType", System.Data.DbType.Int32).Value = (int)toEntityType;
            return Load<EntityRelationship>(controller.ExecuteCollectionCommand(command));
        }
        public static List<EntityRelationship> SelectNotCheckedSince(ICommandController controller, DateTime since)
        {
            DataCommand command = new DataCommand("EntityRelationship", "SelectNotCheckedSince");
            command.AddParameter("CheckedOn", System.Data.DbType.DateTime).Value = since;
            return Load<EntityRelationship>(controller.ExecuteCollectionCommand(command));
        }
        public static List<EntityRelationship> SelectNotCheckedSinceType(ICommandController controller, DateTime since,eESIEntityRelationship relationship)
        {
            DataCommand command = new DataCommand("EntityRelationship", "SelectNotCheckedSinceType");
            command.AddParameter("CheckedOn", System.Data.DbType.DateTime).Value = since;
            command.AddParameter("Relationship", System.Data.DbType.Int32).Value = (int)relationship;
            return Load<EntityRelationship>(controller.ExecuteCollectionCommand(command));
        }

    }
}
