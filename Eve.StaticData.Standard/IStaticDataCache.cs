using Gware.Standard.Storage;
using Gware.Standard.Storage.Controller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.Static.Standard
{
    public interface IStaticDataCache
    {
        ICommandController Controller { get; }
        Task<T> GetItemAsync<T>(int primaryKey) where T : LoadedFromAdapterBase, new();
        T GetItem<T>(int primaryKey) where T : LoadedFromAdapterBase,new();
        Dictionary<int, T> GetItems<T>(IEnumerable<int> primaryKeys) where T : LoadedFromAdapterBase, new();
    }
}
