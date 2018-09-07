namespace Eve.ESIClassMaker.Schema
{
    public class header
    {
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string description { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string type { get; set; }
    }
}
