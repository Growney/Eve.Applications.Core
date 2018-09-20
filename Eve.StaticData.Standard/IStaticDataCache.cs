using Gware.Standard.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.Static.Standard
{
    public interface IStaticDataCache
    {
        Task<T> GetItemAsync<T>(int primaryKey) where T : LoadedFromAdapterBase, new();
        T GetItem<T>(int primaryKey) where T : LoadedFromAdapterBase,new();
        Dictionary<int, T> GetItems<T>(IEnumerable<int> primaryKeys) where T : LoadedFromAdapterBase, new();
    }
}
