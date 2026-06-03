
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace UnityCommander.Core.Performance
{
    internal sealed class ResourceRegistry
    {
        private readonly Dictionary<object, ResourceInfo>
            _resources = new();

        public void Register(ResourceDictionary root)
        {
            Traverse(root, new HashSet<ResourceDictionary>());
        }

        private void Traverse(
            ResourceDictionary dictionary,
            HashSet<ResourceDictionary> visited)
        {
            if (!visited.Add(dictionary))
                return;

            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key is null)
                    continue;

                _resources[entry.Key] = new ResourceInfo
                {
                    Key = entry.Key,
                    DictionarySource =
                        dictionary.Source?.ToString()
                        ?? "<memory>"
                };
            }

            foreach (var merged in dictionary.MergedDictionaries)
            {
                Traverse(merged, visited);
            }
        }

        public bool Contains(object key)
        {
            return _resources.ContainsKey(key);
        }

        public bool TryGetInfo(
            object key,
            out ResourceInfo info)
        {
            return _resources.TryGetValue(key, out info!);
        }

        public IReadOnlyCollection<ResourceInfo>
            GetResources()
        {
            return _resources.Values;
        }
    }
}
