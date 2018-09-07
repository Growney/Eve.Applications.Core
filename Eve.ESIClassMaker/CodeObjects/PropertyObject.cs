using System.Text;

namespace Eve.ESIClassMaker.CodeObjects
{
    public class PropertyObject
    {
        public string APIName { get; set; }
        public string Name
        {
            get
            {
                return CSharpifyName(APIName);
            }
        }
        public string Type { get; set; }
        public bool Required { get; set; }

        public static string CSharpifyName(string name)
        {
            StringBuilder retVal = new StringBuilder();
            char? previous = null;
            for (int i = 0; i < name.Length; i++)
            {
                if(previous != null)
                {
                    if (previous == '_')
                    {
                        retVal.Append(name[i].ToString().ToUpper());
                    }
                    else if (name[i] != '_')
                    {
                        retVal.Append(name[i].ToString().ToLower());
                    }
                }
                else
                {
                    retVal.Append(name[i].ToString().ToUpper());
                }
                previous = name[i];
            }
            return retVal.ToString();
        }

        public PropertyObject(string name,string type,bool required)
        {
            APIName = name;
            Type = type;
            Required = required;
        }

        public override string ToString()
        {
            return $"{Name}:{Type}";
        }

        public string GetDefaultLiteral()
        {
            string retVal = "string.Empty";

            switch (Type.ToLower())
            {
                case "float":
                    retVal = "0.0f";
                    break;
                case "double":
                    retVal = "0.0d";
                    break;
                case "datetime":
                    retVal = "DateTime.MinValue";
                    break;
                case "int64":
                    retVal = "0L";
                    break;
                case "int32":
                    retVal = "0";
                        break;
                case "boolean":
                    retVal = "false";
                    break;
            }

            return retVal;
        }

        public string GetSQLDataType()
        {
            string retVal = "VARCHAR(MAX)";

            switch (Type.ToLower())
            {
                case "float":
                    retVal = "FLOAT";
                    break;
                case "double":
                    retVal = "FLOAT";
                    break;
                case "datetime":
                    retVal = "DATETIME";
                    break;
                case "int64":
                    retVal = "BIGINT";
                    break;
                case "int32":
                    retVal = "INT";
                    break;
                case "boolean":
                    retVal = "BIT";
                    break;
            }

            return retVal;
        }

    }
}
