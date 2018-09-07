using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.dgm
{
    [StaticData("dgmAttributeTypes", "attributeID")]
    public class DgmAttributeTypes : LoadedFromAdapterBase
    {
        public int AttributeID { get; private set; }
        public string AttributeName { get; private set; }
        public string Description { get; private set; }
        public int IconID { get; private set; }
        public float DefaultValue { get; private set; }
        public bool Published { get; private set; }
        public string DisplayName { get; private set; }
        public int UnitID { get; private set; }
        public bool Stackable { get; private set; }
        public bool HighIsGood { get; private set; }
        public int CategoryID { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            AttributeID = adapter.GetValue("attributeID", 0);
            AttributeName = adapter.GetValue("attributeName", string.Empty);
            Description = adapter.GetValue("description", string.Empty);
            IconID = adapter.GetValue("iconID", 0);
            DefaultValue = adapter.GetValue("defaultValue", 0.0f);
            Published = adapter.GetValue("published", false);
            DisplayName = adapter.GetValue("displayName", string.Empty);
            UnitID = adapter.GetValue("unitID", 0);
            Stackable = adapter.GetValue("stackable", false);
            HighIsGood = adapter.GetValue("highIsGood", false);
            CategoryID = adapter.GetValue("categoryID", 0);
        }

        public eve.EveUnits GetUnit(IStaticDataCache cache)
        {
            return cache.GetItem<eve.EveUnits>(UnitID);
        }
        
    }
}