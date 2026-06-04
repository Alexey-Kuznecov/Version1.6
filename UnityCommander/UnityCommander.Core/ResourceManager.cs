
using System;
using System.Collections.Generic;
using System.Windows;
using UnityCommander.Core.Performance;

namespace UnityCommander.Core
{
    public static class ResourceManager
    {
        private static readonly ResourceRegistry _registry = new();
        private static readonly ResourceCache _cache = new();

        public static void Register(ResourceDictionary dictionary)
        {
            _registry.Register(dictionary);
        }

        public static T Get<T>(string key)
        {
            return _cache.GetOrCreate(
                key,
                () =>
                {
                    var resource =
                        Application.Current.TryFindResource(key);

                    if (resource is not T result)
                    {
                        throw new InvalidCastException(
                            $"Resource '{key}' is not of type '{typeof(T).Name}'.");
                    }

                    return result;
                });
        }

        public static T Find<T>(string key)
        {
            var resource =
                Application.Current.TryFindResource(key);
            return (T)resource;
        }

        public static bool Contains(string key)
        {
            return _registry.Contains(key);
        }

        public static bool TryGetInfo(string key, out ResourceInfo info)
        {
            return _registry.TryGetInfo(key, out info);
        }

        public static IReadOnlyCollection<ResourceInfo> GetResources()
        {
            return _registry.GetResources();
        }

        public static CacheStatistics GetStatistics()
        {
            return _cache.GetStatistics();
        }

        public static void ClearCache()
        {
            _cache.Clear();
        }
    }
}
