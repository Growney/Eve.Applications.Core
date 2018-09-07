
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.DataItem.Corporation
{
    [ESIItem("IntegerList", "/corporations/{corporation_id}/members/")]
    public class CorporationMembers : IntegerCollectionItem
    {
        public static System.Threading.Tasks.Task<ESIIntegerCollectionCallResponse<CorporationMembers>> GetMembers(IESIAuthenticationClient client, ICommandController controller, long corporationID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask,bool oldData = false)
        {
            return GetIntegerCollection<CorporationMembers>(client, controller, new Dictionary<string, object>() { { "corporation_id", corporationID } }, authenticationTokenTask,oldData);
        }
    }
}
