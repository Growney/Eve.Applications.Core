using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.eve
{
    [StaticData("eveUnits", "unitID")]
    public class EveUnits : LoadedFromAdapterBase
    {
        public int UnitID { get; private set; }
        public string UnitName { get; private set; }
        public string DisplayName { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            UnitID = adapter.GetValue("unitID", 0);
            UnitName = adapter.GetValue("unitName", string.Empty);
            DisplayName = adapter.GetValue("displayName", string.Empty);
        }
    }
}
