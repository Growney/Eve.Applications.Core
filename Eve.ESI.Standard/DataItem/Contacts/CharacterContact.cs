using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Contacts
{
    [ESIItem("Contact", "/characters/{character_id}/contacts/", typeof(IDIntegerCollectionItem))]
    public class CharacterContact : Contact
    {
        public static Task<ESICollectionCallResponse<CharacterContact>> GetCharacterContacts(IESIAuthenticationClient client, ICommandController controller, long characterID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask, bool oldData = false)
        {
            return GetCollection<CharacterContact>(client, controller, new Dictionary<string, object>() { { "character_id", characterID } }, authenticationTokenTask, oldData);
        }
    }
}
