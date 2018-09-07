using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.chr
{
    [StaticData("chrRaces", "raceID")]
    public class ChrRaces : LoadedFromAdapterBase
    {
        public int RaceID { get; private set; }
        public string RaceName { get; private set; }
        public string Description { get; private set; }
        public int IconID { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            RaceID = adapter.GetValue("raceID", 0);
            RaceName = adapter.GetValue("raceName", string.Empty);
            Description = adapter.GetValue("description", string.Empty);
            IconID = adapter.GetValue("iconID", 0);
        }
    }
}
