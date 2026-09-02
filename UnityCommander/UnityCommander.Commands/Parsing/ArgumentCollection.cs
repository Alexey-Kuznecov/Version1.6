
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Commands.Parsing
{
    public sealed class ArgumentCollection : IArgumentCollection
    {
        private readonly Dictionary<string, List<string>> _named;
        private readonly IReadOnlyList<string> _positional;

        public ArgumentCollection(
            Dictionary<string, List<string>> named,
            IReadOnlyList<string> positional)
        {
            _named = named;
            _positional = positional;
        }

        public bool HasFlag(string name)
            => _named.ContainsKey(name);

        public string? GetString(string name)
            => _named.TryGetValue(name, out var values)
                ? values.FirstOrDefault()
                : null;

        public IReadOnlyList<string> GetStrings(string name)
            => _named.TryGetValue(name, out var values)
                ? values
                : Array.Empty<string>();

        public int GetInt(
            string name,
            int defaultValue = 0)
        {
            return int.TryParse(
                GetString(name),
                out var result)
                    ? result
                    : defaultValue;
        }

        public string? GetAt(int index)
            => index >= 0 && index < _positional.Count
                ? _positional[index]
                : null;
    }
}
