
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public static class DictionaryExtensions
    {
        public static TValue? GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key)
        {
            if (dict == null)
                return default;

            return dict.TryGetValue(key, out var value)
                ? value
                : default;
        }
    }
}
