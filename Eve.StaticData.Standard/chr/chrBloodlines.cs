
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.chr
{
    [StaticData("chrBloodlines", "bloodlineID")]
    public class ChrBloodlines : LoadedFromAdapterBase
    {
        public int BloodlineID { get; private set; }
        public string BloodlineName { get; private set; }
        public int RaceID { get; private set; }
        public string Description { get; private set; }
        public string MaleDescription { get; private set; }
        public string FemaleDescription { get; private set; }
        public int ShipTypeID { get; private set; }
        public int CorporationID { get; private set; }
        public int Perception { get; private set; }
        public int Willpower { get; private set; }
        public int Charisma { get; private set; }
        public int Memory { get; private set; }
        public int Intelligence { get; private set; }
        public int IconID { get; private set; }
        public string ShortDescription { get; private set; }
        public string ShortMaleDescription { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            BloodlineID = adapter.GetValue("bloodlineID", 0);
            BloodlineName = adapter.GetValue("bloodlineName", string.Empty);
            RaceID = adapter.GetValue("raceID", 0);
            Description = adapter.GetValue("description", string.Empty);
            MaleDescription = adapter.GetValue("maleDescription", string.Empty);
            FemaleDescription = adapter.GetValue("femaleDescription", string.Empty);
            ShipTypeID = adapter.GetValue("shipTypeID", 0);
            CorporationID = adapter.GetValue("corporationID", 0);
            Perception = adapter.GetValue("perception", 0);
            Willpower = adapter.GetValue("willpower", 0);
            Charisma = adapter.GetValue("charisma", 0);
            Memory = adapter.GetValue("memory", 0);
            Intelligence = adapter.GetValue("intelligence", 0);
            IconID = adapter.GetValue("iconID", 0);
            ShortDescription = adapter.GetValue("shortDescription", string.Empty);
            ShortMaleDescription = adapter.GetValue("shortMaleDescription", string.Empty);
        }
    }
}
