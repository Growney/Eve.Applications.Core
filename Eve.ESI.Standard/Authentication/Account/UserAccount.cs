using System;
using System.Collections.Generic;
using Eve.ESI.Standard.Token;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.Account
{
    
    public class UserAccount : StoredObjectBase
    {
        public override long Id { get; set; }
        public Guid AccountGuid { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<long> TokenIds { get; set; }

        public UserAccount()
        {
            TokenIds = new List<long>();
        }

        public void AddToken(ICommandController controller, Token.ESIToken token)
        {
            if(token != null && controller != null)
            {
                if(Id == 0)
                {
                    Save(controller);
                }
                if(token.Id == 0)
                {
                    token.Save(controller);
                }

                List<long> ids = new List<long>() { token.Id };
                DataCommand command = new DataCommand("UserAccount", "AddTokens");
                command.AddParameter("Id", System.Data.DbType.UInt64).Value = Id;
                command.AddParameter("TokenIds", "LongIDList").Value = ids.CreateIDList();
                controller.ExecuteQuery(command);
            }
        }
        public List<ESIToken> GetTokenForScope(ICommandController controller, long entityID,eESIEntityType type, eESIScope scope)
        {
            return ESIToken.GetForScope(controller, TokenIds, entityID,type, scope);
        }
        public List<ESIToken> GetTokens(ICommandController controller)
        {
            return ESIToken.GetForID(controller, TokenIds);
        }

        public string GetLink(ICommandController controller, byte type)
        {
            DataCommand command = new DataCommand("AccountLink", "GetLink");

            command.AddParameter("AccountGuid", System.Data.DbType.Guid).Value = AccountGuid;
            command.AddParameter("TypeID", System.Data.DbType.Byte).Value = type;

            return LoadSingle<LoadedFromAdapterValue<string>>(controller.ExecuteCollectionCommand(command))?.Value;
        }

        public override IDataCommand CreateDeleteCommand()
        {
            throw new NotImplementedException();
        }
        public override IDataCommand CreateLoadFromPrimaryKey(long primaryKey)
        {
            DataCommand command = new DataCommand("UserAccount", "Single");
            command.AddParameter("Id", System.Data.DbType.Int64).Value = primaryKey;
            return command;
        }
        public override IDataCommand CreateSaveCommand()
        {
            DataCommand command = new DataCommand("UserAccount", "Save");
            command.AddParameter("Id", System.Data.DbType.Int64).Value = Id;
            command.AddParameter("AccountGuid", System.Data.DbType.Guid).Value = AccountGuid;
            command.AddParameter("CreatedDate", System.Data.DbType.DateTime).Value = CreatedDate;
            command.AddParameter("TokenIds", "LongIDList").Value = TokenIds.CreateIDList();
            return command;
        }
        protected override void OnLoad(IDataAdapter adapter)
        {
            Id = adapter.GetValue("Id", 0L);
            AccountGuid = new Guid(adapter.GetValue("AccountGuid", string.Empty));
            CreatedDate = adapter.GetValue("CreatedDate", DateTime.MinValue);
            TokenIds = Load<LoadedFromAdapterValue<long>>(adapter.Controller.ExecuteCollectionCommand(GetTokenIDSelectCommand())).ToList();
        }
        private DataCommand GetTokenIDSelectCommand()
        {
            DataCommand command = new DataCommand("UserAccount", "TokenIDs");
            command.AddParameter("Id", System.Data.DbType.Int64).Value = Id;
            return command;
        }
        public static UserAccount ForGuid(ICommandController controller,string guid)
        {
            UserAccount retVal = null;
            if(Guid.TryParse(guid,out Guid guidObj))
            {
                DataCommand command = new DataCommand("UserAccount", "ForGuid");
                command.AddParameter("AccountGuid", System.Data.DbType.Guid).Value = guidObj;
                retVal = LoadSingle<UserAccount>(controller.ExecuteCollectionCommand(command));
            }
            return retVal;
        }
        public static UserAccount ForCharacterID(ICommandController controller,long characterID)
        {
            UserAccount retVal = null;
            DataCommand command = new DataCommand("UserAccount", "ForEntity");
            command.AddParameter("EntityID", System.Data.DbType.Int64).Value = characterID;
            command.AddParameter("EntityType", System.Data.DbType.Byte).Value = (byte)eESIEntityType.character;
            retVal = LoadSingle<UserAccount>(controller.ExecuteCollectionCommand(command));
            
            return retVal;
        }

        public static UserAccount CreateNew()
        {
            return new UserAccount()
            {
                CreatedDate = DateTime.UtcNow,
                AccountGuid = Guid.NewGuid()
            };
        }
    }
}
