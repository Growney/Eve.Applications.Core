using Eve.ESI.Standard.DataItem;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve.ESI.Standard
{
    public class ESIIntegerCollectionCallResponse<T> : ESICollectionCallResponse<T> where T : IntegerCollectionItem,new()
    {
        public void SetData(IEnumerable<int> data)
        {
            List<T> store = new List<T>();
            foreach(int item in data)
            {
                T newT = new T
                {
                    CallID = CallID,
                    Value = item
                };
                store.Add(newT);
            }
            Data = store.AsReadOnly();
        }
    }
    public class ESICollectionCallResponse<T> : ESICallResponse where T : ESIItemBase
    {
        public IReadOnlyList<T> Data { get; protected set; }

        public void SetData(IEnumerable<T> data)
        {
            foreach (T item in data)
            {
                item.CallID = CallID;
            }

            Data = data.ToList().AsReadOnly();
        }

        protected override void OnSave(ICommandController controller)
        {
            base.OnSave(controller);
            for (int i = 0; i < (Data?.Count ?? 0); i++)
            {
                Data[i].Save(controller);
            }
        }
        public static void SetStoredData<K>(ICommandController controller, ESICollectionCallResponse<K> response) where K : ESIItemBase, new()
        {
            response.Data = Load<K>(ExecuteSelectDataCommand<K>(controller, response.CallID));
        }
        
        public static E CreateForCode<E, K>(System.Net.HttpStatusCode code) where E : ESICollectionCallResponse<K>, new() where K : ESIItemBase
        {
            return new E()
            {
                Expires = DateTime.MaxValue,
                ResponseCode = code,
                Executed = DateTime.UtcNow
            };
        }
    }
    public class ESICallResponse<T> : ESICallResponse where T : ESIItemBase
    {
        public T Data { get; private set; }
        public bool HasData
        {
            get
            {
                return Data != null;
            }
        }

        public void SetData(T data)
        {
            data.CallID = CallID;
            Data = data;
        }

        public static void SetStoredData<K>(ICommandController controller, ESICallResponse<K> response) where K : ESIItemBase, new()
        {
            response.Data = LoadSingle<K>(ExecuteSelectDataCommand<K>(controller, response.CallID));
        }

        protected override void OnSave(ICommandController controller)
        {
            base.OnSave(controller);
            Data?.Save(controller);
        }

        public static E CreateForCode<E, K>(System.Net.HttpStatusCode code) where E : ESICallResponse<K>, new() where K : ESIItemBase
        {
            return new E()
            {
                Expires = DateTime.MaxValue,
                ResponseCode = code,
                Executed = DateTime.UtcNow
            };
        }
    }
    public class ESICallResponse : StoredObjectBase
    {
        public override long Id { get; set; }
        public Guid CallID { get; set; }
        public Guid ParameterGuid { get; set; }
        public string Uri { get; set; }
        public DateTime Executed { get; set; }
        public System.Net.HttpStatusCode ResponseCode { get; set; }
        public string ETag { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime Expires { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
        public bool Expired
        {
            get
            {
                return Expires < DateTime.UtcNow;
            }
        }
        public bool IsSuccess
        {
            get
            {
                return ResponseCode == System.Net.HttpStatusCode.OK || ResponseCode == System.Net.HttpStatusCode.NotModified;
            }
        }
        public bool ReRun
        {
            get
            {
                return Expired || !IsSuccess;
            }
        }
        public long TimeTaken
        {
            get; internal set;
            
        } 
        public override IDataCommand CreateDeleteCommand()
        {
            throw new NotImplementedException();
        }

        public override IDataCommand CreateLoadFromPrimaryKey(long primaryKey)
        {
            DataCommand command = new DataCommand("ESICallResponse", "Single");
            command.AddParameter("Id", System.Data.DbType.Int64).Value = primaryKey;
            return command;
        }

        public override IDataCommand CreateSaveCommand()
        {
            DataCommand command = new DataCommand("ESICallResponse", "Save");
            command.AddParameter("CallID", System.Data.DbType.Guid).Value = CallID;
            command.AddParameter("ParameterGuid", System.Data.DbType.Guid).Value = ParameterGuid;
            command.AddParameter("Uri", System.Data.DbType.String).Value = Uri;
            command.AddParameter("Executed", System.Data.DbType.DateTime).Value = Executed;
            command.AddParameter("ResponseCode", System.Data.DbType.Int16).Value = (short)ResponseCode;
            command.AddParameter("ETag", System.Data.DbType.String).Value = ETag ?? string.Empty;
            command.AddParameter("LastModified", System.Data.DbType.DateTime).Value = LastModified;
            command.AddParameter("Expires", System.Data.DbType.DateTime).Value = Expires;
            command.AddParameter("Page", System.Data.DbType.Int32).Value = Page;
            command.AddParameter("Pages", System.Data.DbType.Int32).Value = Pages;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            Id = adapter.GetValue("Id", 0L);
            CallID = new Guid(adapter.GetValue("CallID", string.Empty));
            ParameterGuid = new Guid(adapter.GetValue("ParameterGuid", string.Empty));
            Uri = adapter.GetValue("Uri", String.Empty);
            Executed = adapter.GetValue("Executed", DateTime.MinValue);
            ResponseCode = (System.Net.HttpStatusCode)adapter.GetValue("ResponseCode", 0);
            ETag = adapter.GetValue("ETag", string.Empty);
            LastModified = adapter.GetValue("LastModified", DateTime.MinValue);
            Expires = adapter.GetValue("Expires", DateTime.MinValue);
            Page = adapter.GetValue("Page", 0);
            Pages = adapter.GetValue("Pages", 0);
        }

        public static T GetResponseForParameterHash<T>(ICommandController controller, Guid hash) where T : ESICallResponse, new()
        {
            T retVal = null;
            DataCommand command = new DataCommand("ESICallResponse", "ParameterHash");
            command.AddParameter("ParameterGuid", System.Data.DbType.Guid).Value = hash;

            retVal = LoadSingle<T>(controller.ExecuteCollectionCommand(command));

            return retVal;
        }

        
        internal static IDataAdapterCollection ExecuteSelectDataCommand<K>(ICommandController controller, Guid callID)
        {
            DataCommand command = new DataCommand("ESICallResponse", "SelectData");
            command.AddParameter("TableName", System.Data.DbType.String).Value = ESIItemAttribute.GetTableName(typeof(K));
            command.AddParameter("CallID", System.Data.DbType.Guid).Value = callID;
            return controller.ExecuteCollectionCommand(command);
        }

        private static void ExecuteClearDataCommand(ICommandController controller, Type type, Guid callID)
        {
            DataCommand command = new DataCommand("ESICallResponse", "ClearData");
            command.AddParameter("TableName", System.Data.DbType.String).Value = ESIItemAttribute.GetTableName(type);
            command.AddParameter("CallID", System.Data.DbType.Guid).Value = callID;

            controller.ExecuteQuery(command);

            Type[] childTypes = ESIItemAttribute.GetChildTypes(type);
            for (int i = 0; i < childTypes.Length; i++)
            {
                ExecuteClearDataCommand(controller, childTypes[i], callID);
            }
        }

        internal static void ExecuteClearDataCommand<K>(ICommandController controller, Guid callID)
        {
            ExecuteClearDataCommand(controller, typeof(K), callID);
        }

        

        
    }
}
