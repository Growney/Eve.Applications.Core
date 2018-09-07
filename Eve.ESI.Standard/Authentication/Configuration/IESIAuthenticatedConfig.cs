using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.DataItem.Search;
using Gware.Standard.Web.OAuth;

namespace Eve.ESI.Standard.Authentication.Configuration
{
    public interface IESIAuthenticatedConfig : IOAuthConfiguration
    {
        IESIAuthenticationClient Client { get; }
        string ImageUrl { get; }
        string VerifyUrl { get; }
        string ESIUrl { get; }

        string GetImageSource(string type, long id, int size, string fileType = "png");
        string GetImageSource(eSearchEntity entity, long id, int size);
        string GetImageSource(eESIEntityType entity, long id, int size);
    }
}
