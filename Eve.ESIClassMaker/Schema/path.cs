using Eve.ESIClassMaker.CodeObjects;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eve.ESIClassMaker.Schema
{
    public class path
    {
        public string pathString { get; set; }
        public string pathMethod { get; set; }

        public string description { get; set; }
        public List<parameter> parameters { get; set; }
        public Dictionary<int, response> responses { get; set; }
        public string summary { get; set; }
        public List<string> tags { get; set; }

        [JsonProperty(PropertyName = "x-alternate-versions")]
        public List<string> alternateVersions { get; set; }
        [JsonProperty(PropertyName = "x-cached-seconds")]
        public int cachedSeconds { get; set; }
        
        
        public List<ClassObject> GetSuccessClass()
         {
            List<ClassObject> retVal = new List<ClassObject>();

            if (responses.ContainsKey(200))
            {
                response succ = responses[200];
                succ.AddClasses(retVal);
            }

            for (int i = 0; i < retVal.Count; i++)
            {
                retVal[i].Template = pathString;
            }
            return retVal;
        }
    }
}
