
using Gware.Standard.Storage;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Storage.Command;

using System.Collections.Generic;
using System.Reflection;

namespace Eve.Static.Standard
{
    public static class StaticDataLoader
    {
        public static T GetData<T>(ICommandController controller,int primaryKey) where T : LoadedFromAdapterBase,new()
        {
            T retval = null;
            StaticDataAttribute attribute = typeof(T).GetCustomAttribute<StaticDataAttribute>();
            if(attribute != null)
            {
                DataCommand command = new DataCommand("DataLoader", "ForPrimaryKey");
                command.AddParameter("TableName",System.Data.DbType.String).Value = attribute.TableName;
                command.AddParameter("ColumnName", System.Data.DbType.String).Value= attribute.PrimaryKeyColumn;
                command.AddParameter("Value", System.Data.DbType.String).Value = primaryKey;

                retval = LoadedFromAdapterBase.LoadSingle<T>(controller.ExecuteCollectionCommand(command));
            }
            return retval;
        }

        public static List<dgm.dgmTypeAttribute> GetTypeAttributes(ICommandController controller,int typeID)
        {
            DataCommand command = new DataCommand("DataLoader", "TypeAttributes");
            command.AddParameter("Value", System.Data.DbType.String).Value = typeID;

            return LoadedFromAdapterBase.Load<dgm.dgmTypeAttribute>(controller.ExecuteCollectionCommand(command));
        }
    }
}
