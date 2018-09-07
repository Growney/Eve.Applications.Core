namespace Eve.ESIClassMaker.Schema
{
    public class info
    {
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string description { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string title { get; set; }
        [Newtonsoft.Json.JsonProperty(Required = Newtonsoft.Json.Required.DisallowNull)]
        public string version { get; set; }
    }
}
