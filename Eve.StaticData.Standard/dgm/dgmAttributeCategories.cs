using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.dgm
{
    [StaticData("dgmAttributeCategories", "categoryID")]
    public class DgmAttributeCategories : LoadedFromAdapterBase
    {
        public int CategoryID { get; internal set; }
        public string CategoryName { get; internal set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            CategoryID = adapter.GetValue("categoryID", 0);
            CategoryName = adapter.GetValue("categoryName", string.Empty);
        }
    }
}