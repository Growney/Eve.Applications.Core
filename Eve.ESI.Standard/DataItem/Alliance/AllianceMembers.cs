using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Controller;
using System.Collections.Generic;

namespace Eve.ESI.Standard.DataItem.Alliance
{
    [ESIItem("IntegerList", "/alliances/{alliance_id}/corporations/")]
    public class AllianceMembers : IntegerCollectionItem
    {
        public static System.Threading.Tasks.Task<ESIIntegerCollectionCallResponse<AllianceMembers>> GetMembers(IESIAuthenticationClient client, ICommandController controller, long allianceID,bool oldData = false)
        {
            return GetIntegerCollection<AllianceMembers>(client, controller, new Dictionary<string, object>() { { "alliance_id", allianceID } }, alwaysReturnOldData: oldData);
        }
    }
}
