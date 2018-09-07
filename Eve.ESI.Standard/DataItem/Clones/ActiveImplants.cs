using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Clones
{
    [ESIItem("IntegerList", "/characters/{character_id}/implants/")]
    public class ActiveImplants : IntegerCollectionItem
    {
        public static System.Threading.Tasks.Task<ESIIntegerCollectionCallResponse<ActiveImplants>> GetActiveImplants(IESIAuthenticationClient client, ICommandController controller, long characterID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask,bool oldData = false)
        {
            return GetIntegerCollection<ActiveImplants>(client, controller, new Dictionary<string, object>() { { "character_id", characterID } }, authenticationTokenTask,oldData);
        }
    }
}
