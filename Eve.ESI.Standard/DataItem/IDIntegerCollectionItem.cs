using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;

namespace Eve.ESI.Standard.DataItem
{
    [ESIItem("IDIntegerList","")]
    public class IDIntegerCollectionItem : ESIItemBase
    {
        public long Value { get; internal set; }

        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("IDIntegerList", "Save");
            command.AddParameter("Id", System.Data.DbType.Int64).Value = Id;
            command.AddParameter("Value", System.Data.DbType.Int64).Value = Value;
            return command;
        }
        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            Value = adapter.GetValue("Value", 0);
        }
        public static IEnumerable<long> ForIDCallID<T>(ICommandController controller,long id,Guid callID) where T : IDIntegerCollectionItem,new()
        {
            DataCommand command = new DataCommand("IDIntegerList", "ForIdCallID");
            command.AddParameter("CallId", System.Data.DbType.Guid).Value = callID;
            command.AddParameter("Id", System.Data.DbType.Int64).Value = id;

            return Load<T>(controller.ExecuteCollectionCommand(command)).ToIntegerList();
        }
    }

    public static class IDIntegerCollectionItemExtensions
    {
        public static IEnumerable<T> ToIDIntegerCollection<T>(this IEnumerable<long> ids,long id,Guid callID) where T : IDIntegerCollectionItem,new()
        {
            List<T> retVal = new List<T>();
            if(ids != null)
            {
                foreach (int itemID in ids)
                {
                    retVal.Add(new T()
                    {
                        CallID = callID,
                        Value = itemID,
                        Id = id
                    });
                }
            }
            return retVal;
        }

        public static IEnumerable<long> ToIntegerList<T>(this IEnumerable<T> items) where T : IDIntegerCollectionItem
        {
            List<long> retVal = new List<long>();

            foreach(T item in items)
            {
                retVal.Add(item.Value);
            }

            return retVal;
        }
    }
}
