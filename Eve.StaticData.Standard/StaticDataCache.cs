using Gware.Standard.Storage;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.Static.Standard
{
    public class StaticDataCache : IStaticDataCache
    {
        private Dictionary<Type, Dictionary<int, object>> m_cache = new Dictionary<Type, Dictionary<int, object>>();
        public ICommandController Controller { get; }
        private readonly object m_alterLock = new object();
        

        public StaticDataCache(IControllerProvider provider)
        {
            Controller = provider.CreateController("StaticData");
        }

        public async Task<T> GetItemAsync<T>(int primaryKey) where T : LoadedFromAdapterBase, new()
        {
            Type tType = typeof(T);

            if (!CacheContains<T>(primaryKey, out T retVal, tType))
            {
                if (retVal == null)
                {
                    retVal = await GetStoredItemAsync<T>(primaryKey, tType);
                }
            }

            return retVal;
        }

        public T GetItem<T>(int primaryKey) where T : LoadedFromAdapterBase, new()
        {
            Type tType = typeof(T);

            if (!CacheContains<T>(primaryKey, out T retVal, tType))
            {
                if (retVal == null)
                {
                    retVal = GetStoredItem<T>(primaryKey, tType);
                }
            }
            
            return retVal;
        }
        private async Task<T>GetStoredItemAsync<T>(int primaryKey, Type typeOfT = null) where T : LoadedFromAdapterBase, new()
        {
            Type tType = typeOfT ?? typeof(T);
            T retVal = await StaticDataLoader.GetDataAsync<T>(Controller, primaryKey);
            CheckAndAdd(primaryKey, retVal);
            return retVal;
        }
        private T GetStoredItem<T>(int primaryKey, Type typeOfT = null) where T : LoadedFromAdapterBase, new()
        {
            Type tType = typeOfT ?? typeof(T);
            T retVal = StaticDataLoader.GetData<T>(Controller, primaryKey);
            CheckAndAdd(primaryKey,retVal);
            return retVal;
        }

        private void CheckAndAdd<T>(int primaryKey,T value, Type typeOfT = null) where T : LoadedFromAdapterBase, new()
        {
            Type tType = typeOfT ?? typeof(T);
            if (value != null)
            {
                lock (m_alterLock)
                {
                    if (!m_cache.ContainsKey(tType))
                    {
                        m_cache.Add(tType, new Dictionary<int, object>());
                    }

                    if (!m_cache[tType].ContainsKey(primaryKey))
                    {
                        m_cache[tType].Add(primaryKey, value);
                    }

                }
            }
        }

        public bool CacheContains<T>(int primaryKey,out T item,Type typeOfT = null) where T : class
        {
            bool retVal = false;
            item = default;
            Type tType = typeOfT ?? typeof(T);
            lock (m_alterLock)
            {
                if (m_cache.ContainsKey(tType))
                {
                    retVal = m_cache[tType].ContainsKey(primaryKey);
                }
            }
            if (retVal)
            {
                item = m_cache[tType][primaryKey] as T;
            }
            return retVal;
        }

        public Dictionary<int,T> GetItems<T>(IEnumerable<int> primaryKeys) where T : LoadedFromAdapterBase, new()
        {
            Type typeOfT = typeof(T);
            Dictionary<int, T> retVal = new Dictionary<int, T>();
            List<int> loadRequired = new List<int>();
            foreach (int primaryKey in primaryKeys)
            {
                if (!retVal.ContainsKey(primaryKey))
                {
                    if (CacheContains(primaryKey, out T cacheItem, typeOfT))
                    {
                        retVal.Add(primaryKey, cacheItem);
                    }
                    else
                    {
                        loadRequired.Add(primaryKey);
                    }
                }
            }

            foreach(int required in loadRequired)
            {
                if (!retVal.ContainsKey(required))
                {
                    T item = GetItem<T>(required);
                    retVal.Add(required, item);
                }
            }
            return retVal;
        }
    }
}
