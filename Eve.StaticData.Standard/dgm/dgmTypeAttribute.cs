using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using System;

namespace Eve.Static.Standard.dgm
{
    public class dgmTypeAttribute : LoadedFromAdapterBase
    {
        public int TypeID { get; set; }
        public int AttributeID { get; set; }
        public int ValueInt { get; set; }
        public float ValueFloat { get; set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            TypeID = adapter.GetValue("typeID", 0);
            AttributeID = adapter.GetValue("attributeID", 0);
            ValueInt = adapter.GetValue("valueInt", 0);
            ValueFloat = adapter.GetValue("valueFloat", 0.0f);
        }

        public float GetMaxValue()
        {
            return Math.Max(ValueInt, ValueFloat);
        }

        public DgmAttributeTypes GetAttribute(IStaticDataCache cache)
        {
            return cache.GetItem<DgmAttributeTypes>(AttributeID);
        }

        public string ToString(IStaticDataCache cache)
        {
            DgmAttributeTypes attribute = GetAttribute(cache);
            eve.EveUnits unit = attribute.GetUnit(cache);
            return $"{attribute.DisplayName}({attribute.AttributeName}) - {GetMaxValue()}{unit?.DisplayName}";
        }
    }
}
