using Eve.ESIClassMaker.CodeObjects;
using System.Collections.Generic;

namespace Eve.ESIClassMaker.Schema
{
    public class definition
    {
        public string description { get; set; }
        public Dictionary<string,definition> properties { get; set; }
        public List<string> required { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public definition items { get; set; }
        public int maxItems { get; set; }
        public string format { get; set; }

        public PropertyObject AddClasses(string propertyName,List<ClassObject> list)
        {
            bool propRequired = required?.Contains(propertyName) ?? false;
            PropertyObject retVal = null;
            switch (type)
            {

                case "object":
                    ClassObject newObject = new ClassObject(title);
                    list.Add(newObject);
                    foreach (string prop in properties.Keys)
                    {
                        PropertyObject obj = properties[prop].AddClasses(prop, list);
                        if (obj != null)
                        {
                            newObject.Properties.Add(obj);
                        }
                    }
                    retVal = new PropertyObject(propertyName, title, propRequired);
                    break;
                case "array":
                    PropertyObject arrayObj = items.AddClasses(propertyName, list);
                    retVal = new PropertyObject(propertyName, $"List<{arrayObj.Type}>", propRequired);
                    break;
                case "number" when format != "float":
                case "integer":
                    retVal = new PropertyObject(propertyName, format[0].ToString().ToUpper() + format.Substring(1).ToLower(), propRequired);
                    break;
                case "number" when format == "float":
                    retVal = new PropertyObject(propertyName, "Double", propRequired);
                    break;
                case "string" when format == "date-time":
                    retVal = new PropertyObject(propertyName, "DateTime", propRequired);
                    break;
                case "string":
                    retVal = new PropertyObject(propertyName, "String", propRequired);
                    break;
                case "boolean":
                    retVal = new PropertyObject(propertyName, "Boolean", propRequired);
                    break;
                default:
                    retVal = new PropertyObject(propertyName, type, propRequired);
                    break;
            }


            return retVal;
            

        }
    }
}
