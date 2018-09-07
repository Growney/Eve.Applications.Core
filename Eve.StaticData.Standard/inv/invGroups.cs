using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.inv
{
    [StaticData("invGroups", "groupID")]
    public class InvGroups : LoadedFromAdapterBase
    {
        public int GroupID { get; private set; }
        public int CategoryID { get; private set; }
        public string GroupName { get; private set; }
        public int IconID { get; private set; }
        public bool UseBasePrice { get; private set; }
        public bool Anchored { get; private set; }
        public bool Anchorable { get; private set; }
        public bool FittableNonSingleton { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            GroupID = adapter.GetValue("groupID", 0);
            CategoryID = adapter.GetValue("categoryID", 0);
            GroupName = adapter.GetValue("groupName", string.Empty);
            IconID = adapter.GetValue("iconID", 0);
            UseBasePrice = adapter.GetValue("useBasePrice", false);
            Anchored = adapter.GetValue("anchored", false);
            Anchorable = adapter.GetValue("anchorable", false);
            FittableNonSingleton = adapter.GetValue("fittableNonSingleton", false);
        }
    }
}
