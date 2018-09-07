using Gware.Standard.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.ESI.Standard
{
    public class ESICallParameters : ICreatesGuid
    {
        public IReadOnlyDictionary<string, object> TokenValues { get; private set; }
        public IReadOnlyDictionary<string, string> QueryParameters { get; private set; }
        public string Template { get;  set; }
        public string IfNoneMatch { get; set; }
        public eESIDataSource Datasource { get; set; }
        public eESILanguage Language { get; set; }
        public int Page { get; internal set; }
        public string Token { get; internal set; }

        public ESICallParameters(string template,string ifNoneMatch = "",eESIDataSource datasource = eESIDataSource.tranquility,eESILanguage language = eESILanguage.en_us,int page = -1,string token = "",Dictionary<string,object> tokenValue = null, Dictionary<string, string> queryParameters = null)
        {
            Template = template;
            IfNoneMatch = IfNoneMatch;
            TokenValues = tokenValue;
            Datasource = datasource;
            Language = language;
            Page = page;
            Token = token;
            QueryParameters = queryParameters;
        }

        private string ReplaceTemplate(string template)
        {
            string retVal = template;

            if(TokenValues != null)
            {
                foreach (string key in TokenValues.Keys)
                {
                    retVal = retVal.Replace("{" + key + "}", TokenValues[key].ToString());
                }
            }
            
            return retVal;
        }
        private string AddQueryParameters(string addTo)
        {
            StringBuilder retVal = new StringBuilder(addTo);

            if(addTo.Length > 0 && addTo[addTo.Length - 1] != '/')
            {
                retVal.Append('/');
            }

            retVal.Append($"?datasource={Datasource.ToString()}&language={GetLanguageString(Language)}");
            if(Page > 0)
            {
                retVal.Append($"&page={Page}");
            }


            if (QueryParameters != null)
            {
                foreach (KeyValuePair<string, string> item in QueryParameters)
                {
                    retVal.Append($"&{item.Key}={item.Value}");
                }
            }
            return retVal.ToString();

        }
        private string GetLanguageString(eESILanguage lang)
        {
            string retVal = string.Empty;
            switch (lang)
            {
                case eESILanguage.en_us:
                    retVal = "en-us";
                    break;
                default:
                    retVal = lang.ToString();
                    break;
            }
            return retVal;
        }
        private string GetParametersUrl(string template)
        {
            string retVal = ReplaceTemplate(template);
            retVal = AddQueryParameters(retVal);
            return retVal;

        }
        public string GetParametersUrl()
        {
            return GetParametersUrl(Template);
        }
        public void SetHeaders(System.Net.Http.HttpClient client)
        {
            if (!String.IsNullOrWhiteSpace(Token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            if (!String.IsNullOrWhiteSpace(IfNoneMatch))
            {
                client.DefaultRequestHeaders.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue(IfNoneMatch));
            }
        }
        
        public string GetGuidString()
        {
            StringBuilder parameterString = new StringBuilder();
            if (TokenValues != null)
            {
                int count = 0;
                foreach (KeyValuePair<string,object> item in TokenValues)
                {
                    if (count != 0)
                    {
                        parameterString.Append(":");
                    }
                    parameterString.Append($"{item.Key}-{item.Value.ToString()}");
                    count++;
                }
            }

            if(QueryParameters != null)
            {
                int count = 0;
                foreach (KeyValuePair<string,string> item in QueryParameters)
                {
                    if (count != 0)
                    {
                        parameterString.Append(":");
                    }
                    parameterString.Append($"{item.Key}-{item.Value}");
                    count++;
                }

            }

            return $"{parameterString}-{Template}-{Datasource}-{Language}-{Page}";
        }
    }
}
