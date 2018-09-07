using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem.Wars
{
    [ESIItem("IntegerList", "/wars/")]
    public class Wars : IntegerCollectionItem
    {
        public static System.Threading.Tasks.Task<ESIIntegerCollectionCallResponse<Wars>> GetWars(IESIAuthenticationClient client, ICommandController controller, long allianceID, bool oldData = false)
        {
            return GetIntegerCollection<Wars>(client, controller, alwaysReturnOldData: oldData);
        }
    }
}
