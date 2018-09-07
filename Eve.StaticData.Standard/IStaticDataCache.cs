using Gware.Standard.Storage;
using System.Collections.Generic;

namespace Eve.Static.Standard
{
    public interface IStaticDataCache
    {
        T GetItem<T>(int primaryKey) where T : LoadedFromAdapterBase,new();
        Dictionary<int, T> GetItems<T>(IEnumerable<int> primaryKeys) where T : LoadedFromAdapterBase, new();
    }
}
