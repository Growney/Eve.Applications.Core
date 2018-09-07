using Eve.ESIClassMaker.CodeObjects;
using System.Collections.Generic;

namespace Eve.ESIClassMaker.Schema
{
    public class response
    {
        public string description { get; set; }
        public Dictionary<string,header> headers { get; set; }
        public Dictionary<int,definition> responses { get; set; }
        public definition schema { get; set; }

        public void AddClasses(List<ClassObject> list)
        {
            schema.AddClasses(string.Empty,list);
        }
    }
}
