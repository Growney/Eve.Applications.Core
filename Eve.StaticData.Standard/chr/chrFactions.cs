
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.chr
{
    [StaticData("chrFactions", "factionID")]
    public class ChrFactions : LoadedFromAdapterBase
    {
        public int FactionID { get; private set; }
        public string FactionName { get; private set; }
        public string Description { get; private set; }
        public int RaceIDs { get; private set; }
        public int SolarSystemID { get; private set; }
        public int CorporationID { get; private set; }
        public float SizeFactor { get; private set; }
        public int StationCount { get; private set; }
        public int StationSystemCount { get; private set; }
        public int MilitiaCorporationID { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            FactionID = adapter.GetValue("factionID", 0);
            FactionName = adapter.GetValue("factionName", string.Empty);
            Description = adapter.GetValue("description", string.Empty);
            RaceIDs = adapter.GetValue("raceIDs", 0);
            SolarSystemID = adapter.GetValue("solarSystemID", 0);
            CorporationID = adapter.GetValue("corporationID", 0);
            SizeFactor = adapter.GetValue("sizeFactor", 0.0f);
            StationCount = adapter.GetValue("stationCount", 0);
            StationSystemCount = adapter.GetValue("stationSystemCount", 0);
            MilitiaCorporationID = adapter.GetValue("militiaCorporationID", 0);
        }
    }
}
