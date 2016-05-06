using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using PagedList;

namespace BeIT.MemCached.Helpers
{
    public static class BeItMemCachedClientExtensions
    {
        private readonly static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool TryLookup<T>(this MemcachedClient cache, string key, out T value)
        {
            value = default(T);
            if (String.IsNullOrEmpty(key))
                return false;

            string elementKey;
            if (!cache.TryGet(key, out elementKey))
                return false;

            return cache.TryGet(elementKey, out value);
        }

        public static bool TryGet<T>(this MemcachedClient cache, string key, out T value)
        {
            value = default(T);

            if (String.IsNullOrEmpty(key))
                return false;

            try
            {
                dynamic result = cache.Get(key);

                if (!(result is T))
                    return false;

                value = (T)result;
                return true;
            }
            catch (Exception e)
            {
                logger.Warn(e.Message, e);
                return false;
            }
        }


        public static IPagedList<T> SelectMany<T>(this MemcachedClient cache, IEnumerable<string> keys, IPagedList metaData)
        {
            IEnumerable<T> entities = cache.SelectMany<T>(keys);
            if (metaData == null)
                return new StaticPagedList<T>(entities, 1, entities.Count(), entities.Count());

            return new StaticPagedList<T>(entities, metaData);
        }

        public static IEnumerable<T> SelectMany<T>(this MemcachedClient cache, IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");

            return cache.SelectMany<T>(keys.ToArray());
        }

        public static IEnumerable<T> SelectMany<T>(this MemcachedClient cache, string[] keys)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            if (keys == null)
                throw new ArgumentNullException("keys");

            object[] results = cache.Get(keys);

            return results.OfType<T>();
        }
    }
}
