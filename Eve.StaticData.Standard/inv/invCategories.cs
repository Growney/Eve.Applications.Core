using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.inv
{
    public class InvCategories : LoadedFromAdapterBase
    {
        public int CategoryID { get; private set; }
        public string CategoryName { get; private set; }
        public int IconID { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            CategoryID = adapter.GetValue("CategoryID", 0);
            CategoryName = adapter.GetValue("CategoryName", string.Empty);
            IconID = adapter.GetValue("IconID", 0);
        }
    }
}
