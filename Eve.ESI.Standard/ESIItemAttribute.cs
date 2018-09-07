using System;
using System.Reflection;

namespace Eve.ESI.Standard
{
    internal class ESIItemAttribute : Attribute
    {
        public string Template { get; private set; }
        public string TableName { get; private set; }
        public Type[] ChildTypes { get; private set; }

        public ESIItemAttribute(string tableName,string template,params Type[] childTypes)
        {
            TableName = tableName;
            Template = template;
            ChildTypes = childTypes;
        }

        public static string GetTableName(Type type)
        {
            
            ESIItemAttribute att = type.GetCustomAttribute<ESIItemAttribute>();
            if(att != null)
            {
                return att.TableName;
            }
            else
            {
                return type.Name;
            }
        }
        public static string GetRouteTemplate(Type type)
        {
            ESIItemAttribute att = type.GetCustomAttribute<ESIItemAttribute>();
            if (att != null)
            {
                return att.Template;
            }
            else
            {
                return string.Empty;
            }
        }

        public static Type[] GetChildTypes(Type type)
        {
            ESIItemAttribute att = type.GetCustomAttribute<ESIItemAttribute>();
            if(att != null)
            {
                return att.ChildTypes ?? new Type[0];
            }
            else
            {
                return new Type[0];
            }
        }
    }
}
