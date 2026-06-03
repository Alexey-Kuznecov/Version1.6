
using System;
using System.Windows;

namespace UnityCommander.Core.Performance
{
    public sealed class ResourceInfo
    {
        public object Key { get; init; }

        public string DictionarySource { get; init; } = string.Empty;

        public ResourceDictionary Dictionary { get; init; } = null!;

        public Type ResourceType { get; init; } = typeof(object);

        public int Layer { get; init; }
    }
}
