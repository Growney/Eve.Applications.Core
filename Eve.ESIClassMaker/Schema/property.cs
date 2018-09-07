using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eve.ESIClassMaker.Schema
{
    public class property
    {
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string format { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string title { get; set; }
        [JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull, PropertyName = "enum")]
        public List<string> enumValues { get; set; }

        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string description { get; set; }

        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string type { get; set; }
    }
}
