using System;

namespace Eve.Static.Standard
{
    internal class StaticDataAttribute : Attribute
    {
        public string TableName { get; }
        public string PrimaryKeyColumn { get; }

        public StaticDataAttribute(string tableName,string primaryKeyColumn)
        {
            TableName = tableName;
            PrimaryKeyColumn = primaryKeyColumn;
        }
    }
}
