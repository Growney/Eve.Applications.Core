
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Contacts
{
    [ESIItem("Contact", "/corporations/{corporation_id}/contacts/", typeof(IDIntegerCollectionItem))]
    public class CorporationContact : Contact
    {
        public static Task<ESICollectionCallResponse<CorporationContact>> GetCorporationContacts(IESIAuthenticationClient client,ICommandController controller,int corporationID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask,bool oldData = false)
        {
            return GetCollection<CorporationContact>(client, controller, new Dictionary<string, object>() { { "corporation_id", corporationID } }, authenticationTokenTask, oldData);
        } 
    }
}
