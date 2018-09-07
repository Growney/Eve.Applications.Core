using System.Collections.Generic;

namespace Eve.ESIClassMaker.Schema
{
    public class securityDefinition
    {
        
        public string authorizationUrl { get; set; }
        
        public string flow { get; set; }
        
        public Dictionary<string,string> scopes { get; set; }
        
        public string type { get; set; }
    }
}
