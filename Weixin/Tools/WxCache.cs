using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Weixin.Tools
{
    public class WxCache
    {
        private static Cache _cache;

        public static object Get(string key)
        {
            _cache = HttpRuntime.Cache;
            return _cache.Get(key);
        }

        public static void Set(string key, object value)
        {
            _cache = HttpRuntime.Cache;
            _cache.Insert(key, value);
        }

        public static void Set(string key, object value, int ts)
        {
            _cache = HttpRuntime.Cache;
            _cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(ts));
        }

        public static void Remove(string key)
        {
            _cache = HttpRuntime.Cache;
            _cache.Remove(key);
        }
    }
}
