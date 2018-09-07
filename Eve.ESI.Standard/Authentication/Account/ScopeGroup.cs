using Eve.ESI.Standard.Authentication;

namespace Eve.ESI.Standard.Account
{
    public class ScopeGroup
    {
        public ulong Value { get; }
        public string Name { get; }

        public eESIScope[] Scopes { get; }

        public string Description { get; }

        public bool Required { get; }

        public ScopeGroup(ulong value,string name, string description = "", params eESIScope[] scopes)
            :this(value,name,false,description,scopes)
        {
        }
        public ScopeGroup(ulong value, string name, bool required, string description = "", params eESIScope[] scopes)
        {
            Value = value;
            Name = name;
            Scopes = scopes;
            Description = description;
            Required = required;
        }

        public string GetScopeString()
        {
            return ESIScopeHelper.GetScopeString(Scopes);
        }

        public ScopeGroup SetRequired(bool required)
        {
            return new ScopeGroup(Value,Name, required, Description, Scopes);
        }
    }
}
