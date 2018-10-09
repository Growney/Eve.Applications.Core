
using System;
using System.Collections.Generic;

using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.Account
{
    public class LinkedUserAccount : UserAccount
    {
        public string Link { get; set; }
        public byte TypeID { get; set; }

        public LinkedUserAccount()
        {
           
        }

        public LinkedUserAccount(List<long> tokenIds)
            :base(tokenIds)
        {

        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            Link = adapter.GetValue("Link", string.Empty);
            TypeID = adapter.GetValue("TypeID", (byte)0);
        }

        public static LinkedUserAccount CreateLink(ICommandController controller, UserAccount account, byte type, string link)
        {
            DataCommand command = new DataCommand("AccountLink", "Link");

            command.AddParameter("AccountGuid", System.Data.DbType.Guid).Value = account.AccountGuid;
            command.AddParameter("TypeID", System.Data.DbType.Byte).Value = type;
            command.AddParameter("Link", System.Data.DbType.String).Value = link;

            controller.ExecuteQuery(command);

            return CreateObject(link, type, account);
        }

        public static void RemoveLink(ICommandController controller, Guid accountGuid, byte type)
        {
            DataCommand command = new DataCommand("AccountLink", "Remove");

            command.AddParameter("AccountGuid", System.Data.DbType.Guid).Value = accountGuid;
            command.AddParameter("TypeID", System.Data.DbType.Byte).Value = type;

            controller.ExecuteQuery(command);
        }

        public static LinkedUserAccount ForLink(ICommandController controller, string link)
        {
            DataCommand command = new DataCommand("AccountLink", "ForLink");

            command.AddParameter("Link", System.Data.DbType.String).Value = link;

            return LoadSingle<LinkedUserAccount>(controller.ExecuteCollectionCommand(command));
        }

        public static List<LinkedUserAccount> AllLinked(ICommandController controller, byte type)
        {
            DataCommand command = new DataCommand("AccountLink", "AllLinked");
            command.AddParameter("TypeID", System.Data.DbType.Byte).Value = type;
            return Load<LinkedUserAccount>(controller.ExecuteCollectionCommand(command));
        }

        public static LinkedUserAccount CreateObject(string link,byte typeID,UserAccount account)
        {
            return new LinkedUserAccount(account.TokenIds)
            {
                Id = account.Id,
                AccountGuid = account.AccountGuid,
                CreatedDate = account.CreatedDate,
                Link = link,
                TypeID = typeID
            };
        }
    }
}
