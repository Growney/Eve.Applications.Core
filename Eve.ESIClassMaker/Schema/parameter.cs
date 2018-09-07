using Newtonsoft.Json;

namespace Eve.ESIClassMaker.Schema
{
    public class parameter
    {
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string description { get; set; }
        [JsonProperty(PropertyName = "in")]
        public string usedIn { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public int minimum { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string name { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public bool required { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string type { get; set; }
    }
}
