
using Gware.Standard.Storage;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Storage.Command;

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Eve.Static.Standard
{
    public static class StaticDataLoader
    {

        private static IDataCommand GetSelectCommand<T>(int primaryKey)
        {
            DataCommand retVal = null;
            StaticDataAttribute attribute = typeof(T).GetCustomAttribute<StaticDataAttribute>();
            if (attribute != null)
            {
                retVal = new DataCommand("DataLoader", "ForPrimaryKey");
                retVal.AddParameter("TableName", System.Data.DbType.String).Value = attribute.TableName;
                retVal.AddParameter("ColumnName", System.Data.DbType.String).Value = attribute.PrimaryKeyColumn;
                retVal.AddParameter("Value", System.Data.DbType.String).Value = primaryKey;
            }
            return retVal;
        }
        public async static Task<T> GetDataAsync<T>(ICommandController controller,int primaryKey) where T : LoadedFromAdapterBase, new()
        {
            return LoadedFromAdapterBase.LoadSingle<T>(await controller.ExecuteCollectionCommandAsync(GetSelectCommand<T>(primaryKey)));
        }
        public static T GetData<T>(ICommandController controller,int primaryKey) where T : LoadedFromAdapterBase,new()
        {
            return LoadedFromAdapterBase.LoadSingle<T>(controller.ExecuteCollectionCommand(GetSelectCommand<T>(primaryKey)));
        }

        public static List<dgm.dgmTypeAttribute> GetTypeAttributes(ICommandController controller,int typeID)
        {
            DataCommand command = new DataCommand("DataLoader", "TypeAttributes");
            command.AddParameter("Value", System.Data.DbType.String).Value = typeID;

            return LoadedFromAdapterBase.Load<dgm.dgmTypeAttribute>(controller.ExecuteCollectionCommand(command));
        }
    }
}
