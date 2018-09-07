using System.Collections.Generic;
using System.Text;

namespace Eve.ESIClassMaker.CodeObjects
{
    public class ClassObject
    {
        public string Template { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public List<PropertyObject> Properties { get; }

        public ClassObject(string name)
        {
            Name = name;
            Properties = new List<PropertyObject>();
        }

        public override string ToString()
        {
            return Name;
        }

        public string GetCSharp()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using Gware.Standard.Storage.Adapter;");
            builder.AppendLine("using Gware.Standard.Storage.Command;");
            builder.AppendLine("using Gware.Standard.Storage.Controller;");
            builder.AppendLine();
            builder.AppendLine("namespace Eve.ESI.Standard.DataItem");
            builder.AppendLine("{");
            builder.AppendLine($"[ESIItem(\"{Name}\", \"{Template}\")]");
            builder.AppendLine($"public class {Name} : ESIItemBase");
            builder.AppendLine("{");
            builder.AppendLine();
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyObject obj = Properties[i];
                builder.AppendLine($"[Newtonsoft.Json.JsonProperty(PropertyName = \"{obj.APIName}\")]");
                builder.AppendLine($"public {obj.Type} {obj.Name} " + "{ get; private set; }");
            }
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("public override IDataCommand OnCreateSaveCommand()");
            builder.AppendLine("{");
            builder.AppendLine($"DataCommand command = new DataCommand(\"{Name}\", \"Save\");");
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyObject obj = Properties[i];
                builder.AppendLine($"command.AddParameter(\"{obj.Name}\", System.Data.DbType.{obj.Type}).Value = {obj.Name};");
            }
            builder.AppendLine("return command;");
            builder.AppendLine("}");

            builder.AppendLine();
            builder.AppendLine("protected override void OnLoad(IDataAdapter adapter)");
            builder.AppendLine("{");
            builder.AppendLine("base.OnLoad(adapter);");
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyObject obj = Properties[i];
                builder.AppendLine($"{obj.Name} = adapter.GetValue(\"{obj.Name}\", {obj.GetDefaultLiteral()} );");
            }
            builder.AppendLine("}");


            builder.AppendLine("}");
            builder.AppendLine("}");
            return builder.ToString();
        }

        public string GetSQLTable()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"CREATE TABLE [dbo].[{Name}]");
            builder.AppendLine("(");
            builder.AppendLine("\t[CallID] UNIQUEIDENTIFIER,");
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyObject obj = Properties[i];
                string nullString = (obj.Required) ? "NOT NULL" : "NULL";
                builder.Append($"\t[{obj.Name}] {obj.GetSQLDataType()} {nullString}");
                if (i == Properties.Count - 1)
                {
                    builder.AppendLine();
                }
                else
                {
                    builder.AppendLine(",");
                }
            }

            builder.AppendLine(")");
            builder.AppendLine("GO");
            builder.AppendLine($"CREATE CLUSTERED INDEX IX_{Name}_CallID ON dbo.[{Name}] (CallID)");

            return builder.ToString();
        }

        public string GetSQLStoredProcedure()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"CREATE PROCEDURE [dbo].[p{Name}]");
            builder.AppendLine("\t@Result NCHAR(50),");
            builder.AppendLine("\t@CallID UNIQUEIDENTIFIER,");
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyObject obj = Properties[i];
                builder.Append($"\t@{obj.Name} {obj.GetSQLDataType()} = NULL");
                if (i != Properties.Count - 1)
                {
                    builder.AppendLine(",");
                }
                else
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine("AS");
            builder.AppendLine("BEGIN");
            builder.AppendLine();
            builder.AppendLine("IF @Result = 'Save'");
            builder.AppendLine("BEGIN");


            StringBuilder propBuilder = new StringBuilder();
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyObject obj = Properties[i];
                if (i != 0)
                {
                    propBuilder.Append(",");
                }
                propBuilder.Append($"{obj.Name}");

            }
            builder.AppendLine($"\tINSERT INTO {Name} (CallID,{propBuilder.ToString()})");
            propBuilder.Clear();
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyObject obj = Properties[i];
                if (i != 0)
                {
                    propBuilder.Append(",");
                }
                propBuilder.Append($"@{obj.Name}");
            }
            builder.AppendLine($"\tVALUES (@CallID,{propBuilder.ToString()})");
            builder.AppendLine();
            builder.AppendLine("SELECT 0 AS [Value]");
            builder.AppendLine("END");

            builder.AppendLine("IF @Result = 'All'");
            builder.AppendLine("BEGIN");
            builder.AppendLine($"\tSELECT* FROM {Name}");
            builder.AppendLine("END");

            builder.AppendLine("IF @Result = 'ForRequest'");
            builder.AppendLine("BEGIN");
            builder.AppendLine($"\tSELECT* FROM {Name} WHERE CallID = @CallID");
            builder.AppendLine("END");
            builder.AppendLine();
            builder.AppendLine("END");

            return builder.ToString();
        }
    }
}
