using Eve.Static.Standard.dgm;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using System.Collections.Generic;

namespace Eve.Static.Standard.inv
{
    [StaticData("invTypes", "typeID")]
    public class InvTypes : LoadedFromAdapterBase
    {
        public int TypeID { get; private set; }
        public int GroupID { get; private set; }
        public string TypeName { get; private set; }
        public string Description { get; private set; }
        public float Mass { get; private set; }
        public float Volume { get; private set; }
        public float Capacity { get; private set; }
        public int PortionSize { get; private set; }
        public int RaceID { get; private set; }
        public decimal BasePrice { get; private set; }
        public bool Published { get; private set; }
        public int MarketGroupID { get; private set; }
        public int IconID { get; private set; }
        public int SoundID { get; private set; }

        public IReadOnlyList<dgm.dgmTypeAttribute> AttributeValues { get; private set; }
        
        public InvTypes()
        {
            AttributeValues = new List<dgm.dgmTypeAttribute>();
        }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            TypeID = adapter.GetValue("typeID", 0);
            GroupID = adapter.GetValue("groupID", 0);
            TypeName = adapter.GetValue("typeName", string.Empty);
            Description = adapter.GetValue("description", string.Empty);
            Mass = adapter.GetValue("mass", 0.0f);
            Volume = adapter.GetValue("volume", 0.0f);
            Capacity = adapter.GetValue("capacity", 0.0f);
            PortionSize = adapter.GetValue("portionSize", 0);
            RaceID = adapter.GetValue("raceID", 0);
            BasePrice = adapter.GetValue("basePrice", 0.0m);
            Published = adapter.GetValue("published", false);
            MarketGroupID = adapter.GetValue("marketGroupID", 0);
            IconID = adapter.GetValue("iconID", 0);
            SoundID = adapter.GetValue("soundID", 0);
            
            AttributeValues = StaticDataLoader.GetTypeAttributes(adapter.Controller, TypeID);
        }



        public Dictionary<int, dgm.dgmTypeAttribute> GetTypeGroupedAttributes()
        {
            Dictionary<int, dgm.dgmTypeAttribute>  retVal = new Dictionary<int,dgm.dgmTypeAttribute>();
            foreach(dgm.dgmTypeAttribute att in AttributeValues)
            {
                retVal.Add(att.AttributeID, att);
            }
            return retVal;
        }

        public Dictionary<DgmAttributeCategories,List<dgmTypeAttribute>> GetCategoryGroupedAttributes(IStaticDataCache cache)
        {
            Dictionary<DgmAttributeCategories, List<dgmTypeAttribute>> retVal = new Dictionary<DgmAttributeCategories, List<dgmTypeAttribute>>();
            Dictionary<int, DgmAttributeCategories> categories = new Dictionary<int, DgmAttributeCategories>();
            foreach (dgm.dgmTypeAttribute att in AttributeValues)
            {
                DgmAttributeTypes attributeType = att.GetAttribute(cache);
                if (!categories.ContainsKey(attributeType.CategoryID))
                {
                    DgmAttributeCategories category = cache.GetItem<DgmAttributeCategories>(attributeType.CategoryID);
                    if(category == null)
                    {
                        category = new DgmAttributeCategories()
                        {
                            CategoryID = 0,
                            CategoryName = "Unknown"
                        };
                    }
                    retVal.Add(category, new List<dgmTypeAttribute>());
                    categories.Add(attributeType.CategoryID, category);
                }
                retVal[categories[attributeType.CategoryID]].Add(att);
            }
            return retVal;
        }
    }
}
