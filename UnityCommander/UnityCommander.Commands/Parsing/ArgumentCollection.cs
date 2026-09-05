
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UnityCommander.Commands.Parsing
{
    public sealed class ArgumentCollection : IArgumentCollection
    {
        private readonly Dictionary<string, List<string>> _named;
        private readonly List<string> _positional;

        private readonly IArgumentValueParser _valueParser;
        private readonly IKeyValueParser _keyValueParser;

        public ArgumentCollection(
            Dictionary<string, List<string>> named,
            List<string> positional,
            IArgumentValueParser valueParser = null,
            IKeyValueParser keyValueParser = null)
        {
            _named = named;
            _positional = positional;
            _valueParser = valueParser ?? new ArgumentValueParser();
            _keyValueParser = keyValueParser ?? new KeyValueParser();
        }

        public bool HasFlag(string name)
        {
            return _named.ContainsKey(name);
        }

        public string? GetString(string name)
        {
            return _named.TryGetValue(name, out var values)
                ? values.FirstOrDefault()
                : null;
        }

        public IReadOnlyList<string> GetValues(string name)
        {
            return _named.TryGetValue(name, out var values)
                ? values
                : Array.Empty<string>();
        }

        public IReadOnlyList<string> GetStrings(string name)
        {
            return GetValues(name)
                .SelectMany(_valueParser.Parse)
                .ToArray();
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

        public bool TryGetKeyValues(
          string name,
          out IReadOnlyList<KeyValuePair<string, object>> values)
        {
            var result = new List<KeyValuePair<string, object>>();

            foreach (var rawValue in GetValues(name))
            {
                if (_keyValueParser.TryParse(
                    rawValue,
                    out var key,
                    out var value))
                {
                    result.Add(
                        new KeyValuePair<string, object>(
                            key,
                            ParseValue(value)));
                }
            }

            values = result;
            return result.Count > 0;
        }

        public string? GetAt(int index)
        {
            return index >= 0 && index < _positional.Count
                ? _positional[index]
                : null;
        }

        public IReadOnlyList<string> Positional =>
            _positional;

        private static object ParseValue(string value)
        {
            if (bool.TryParse(value, out var boolean))
                return boolean;

            if (int.TryParse(value, out var integer))
                return integer;

            if (double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var number))
            {
                return number;
            }

            return value;
        }
    }
}
