

using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;

namespace Eve.Static.Standard.chr
{
    [StaticData("chrAncestries", "ancestryID")]
    public class ChrAncestries : LoadedFromAdapterBase
    {
        public int AncestryID { get; private set; }
        public string AncestryName { get; private set; }
        public int BloodlineID { get; private set; }
        public string Description { get; private set; }
        public int Perception { get; private set; }
        public int Willpower { get; private set; }
        public int Charisma { get; private set; }
        public int Memory { get; private set; }
        public int Intelligence { get; private set; }
        public int IconID { get; private set; }

        protected override void LoadFrom(IDataAdapter adapter)
        {
            AncestryID = adapter.GetValue("ancestryID", 0);
            AncestryName = adapter.GetValue("ancestryName", string.Empty);
            BloodlineID = adapter.GetValue("bloodlineID", 0);
            Description = adapter.GetValue("description", string.Empty);
            Perception = adapter.GetValue("perception", 0);
            Willpower = adapter.GetValue("willpower", 0);
            Charisma = adapter.GetValue("charisma", 0);
            Memory = adapter.GetValue("memory", 0);
            Intelligence = adapter.GetValue("intelligence", 0);
            IconID = adapter.GetValue("iconID", 0);
        }
    }
}
