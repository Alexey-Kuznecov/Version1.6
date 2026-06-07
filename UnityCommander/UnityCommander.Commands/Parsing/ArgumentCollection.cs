
using System;
using System.Collections.Generic;

namespace UnityCommander.Commands.Parsing
{
    public sealed class ArgumentCollection : IArgumentCollection
    {
        private readonly Dictionary<string, string> _arguments;
        private readonly IReadOnlyList<string> _positional;

        public ArgumentCollection(
            Dictionary<string, string> arguments,
            IReadOnlyList<string> positional)
        {
            _arguments = arguments;
            _positional = positional;
        }

        public bool HasFlag(string name)
        {
            return _arguments.ContainsKey(name);
        }

        public string? GetString(string name)
        {
            return _arguments.TryGetValue(name, out var value)
                ? value
                : null;
        }

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
        {
            return index >= 0 && index < _positional.Count
                ? _positional[index]
                : null;
        }
    }
}
