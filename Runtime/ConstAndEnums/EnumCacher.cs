using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//https://www.meziantou.net/caching-enum-tostring-to-improve-performance.htm
internal static class EnumCacher
{
    private static readonly ConcurrentDictionary<AiContextKey, string> s_cache = new ConcurrentDictionary<AiContextKey, string>();

    internal static string ToStringCached(this AiContextKey value)
    {
        return s_cache.GetOrAdd(value, v => v.ToString());
    }
}
