using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace ProjectEye.Core.Service
{
    public class CacheService : IService
    {
        private readonly ObjectCache cache;
        public CacheService()
        {
            cache = MemoryCache.Default;
        }
        public void Init()
        {

        }
        public object this[string key]
        {
            get
            {
                var result = cache.Get(key);
                if (result != null)
                {
                    cache.Remove(key);
                }
                return result;
            }
            set
            {
                cache.Set(key, value, null);
            }
        }
    }
}
