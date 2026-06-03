
using System;
using System.Collections.Generic;
namespace UnityCommander.Core.Performance
{
    internal sealed class ResourceCache
    {
        private readonly Dictionary<string, object> _cache = new();

        private long _hits;
        private long _misses;

        public T GetOrCreate<T>(
            string key,
            Func<T> factory)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                _hits++;
                return (T)cached;
            }

            _misses++;

            var value = factory();

            _cache[key] = value!;

            return value;
        }

        public CacheStatistics GetStatistics()
        {
            return new CacheStatistics
            {
                Count = _cache.Count,
                Hits = _hits,
                Misses = _misses
            };
        }

        public void Clear()
        {
            _cache.Clear();
            _hits = 0;
            _misses = 0;
        }
    }
}
