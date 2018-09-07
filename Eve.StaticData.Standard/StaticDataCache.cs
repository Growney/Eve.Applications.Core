using Gware.Standard.Storage;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;


namespace Eve.Static.Standard
{
    public class StaticDataCache : IStaticDataCache
    {
        private Dictionary<Type, Dictionary<int, object>> m_cache = new Dictionary<Type, Dictionary<int, object>>();
        private ICommandController m_controller;
        private object m_alterLock = new object();

        public StaticDataCache(IControllerProvider provider)
        {
            m_controller = provider.CreateController("StaticData");
        }

        public T GetItem<T>(int primaryKey) where T : LoadedFromAdapterBase, new()
        {
            T retVal;
            Type tType = typeof(T);

            if(!CacheContains<T>(primaryKey, out retVal, tType))
            {
                if (retVal == null)
                {
                    retVal = GetStoredItem<T>(primaryKey, tType);
                }
            }
            
            return retVal;
        }

        private T GetStoredItem<T>(int primaryKey, Type typeOfT = null) where T : LoadedFromAdapterBase, new()
        {
            Type tType = typeOfT ?? typeof(T);
            T retVal = StaticDataLoader.GetData<T>(m_controller, primaryKey);
            if (retVal != null)
            {
                lock (m_alterLock)
                {
                    if (!m_cache.ContainsKey(tType))
                    {
                        m_cache.Add(tType, new Dictionary<int, object>());
                    }

                    if (!m_cache[tType].ContainsKey(primaryKey))
                    {
                        m_cache[tType].Add(primaryKey, retVal);
                    }
                    
                }
            }
            return retVal;
        }

        public bool CacheContains<T>(int primaryKey,out T item,Type typeOfT = null) where T : class
        {
            bool retVal = false;
            item = default(T);
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
