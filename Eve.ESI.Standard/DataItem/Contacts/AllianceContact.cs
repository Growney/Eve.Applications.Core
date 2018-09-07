using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Contacts
{
    [ESIItem("Contact", "/alliances/{alliance_id}/contacts/", typeof(IDIntegerCollectionItem))]
    public class AllianceContact : Contact
    {
        public static Task<ESICollectionCallResponse<AllianceContact>> GetAllianceContacts(IESIAuthenticationClient client, ICommandController controller, int allianceID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask, bool oldData = false)
        {
            return GetCollection<AllianceContact>(client, controller, new Dictionary<string, object>() { { "alliance_id", allianceID } }, authenticationTokenTask, oldData);
        }
    }
}
