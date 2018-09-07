using Eve.ESI.Core.Authentication.Client;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.DataItem.Search;
using Microsoft.Extensions.Configuration;

namespace Eve.ESI.Core
{
    public class ESIAuthenticatedConfig : IESIAuthenticatedConfig
    {
        public string ImageUrl => GetConfigSetting("ImageUrl");

        public string VerifyUrl => GetConfigSetting("VerifyURL");

        public string ESIUrl => GetConfigSetting("ESIURL");

        public string AuthUrl => GetConfigSetting("AuthURL");

        public string CallBackUrl => GetConfigSetting("CallbackURL");

        public string ClientID => GetConfigSetting("ClientID");

        public string Secret => GetConfigSetting("Secret");

        public IESIAuthenticationClient Client { get; }

        private readonly IConfiguration m_config;

        public ESIAuthenticatedConfig(IConfiguration config)
        {
            m_config = config;
            Client = new ESIWebClient(ESIUrl,AuthUrl, ClientID, Secret, VerifyUrl);
        }

        private string GetConfigSetting(string key)
        {
            return m_config[$"ESIConfig:{key}"];
        }

        public string GetImageSource(string type, long id, int size, string fileType = "png")
        {
            return $"{ImageUrl}/{type}/{id}_{size}.{fileType}";
        }

        public string GetImageSource(eSearchEntity entity,long id,int size)
        {
            switch (entity)
            {
                case eSearchEntity.alliance:
                case eSearchEntity.faction:
                    return GetImageSource("Alliance", id, size);
                case eSearchEntity.character:
                    return GetImageSource("Character", id, size, "jpg");
                case eSearchEntity.corporation:
                    return GetImageSource("Corporation", id, size);
                case eSearchEntity.inventory_type:
                    return GetImageSource("Type", id, size);
                default:
                    return string.Empty;
            }
        }

        public string GetImageSource(eESIEntityType type,long id,int size)
        {
            switch (type)
            {
                case eESIEntityType.character:
                    return GetImageSource(eSearchEntity.character,id, size);
                case eESIEntityType.corporation:
                    return GetImageSource(eSearchEntity.corporation, id, size);
                case eESIEntityType.faction:
                case eESIEntityType.alliance:
                    return GetImageSource(eSearchEntity.alliance, id, size);
                default:
                    return string.Empty;
            }
        }
    }
}
