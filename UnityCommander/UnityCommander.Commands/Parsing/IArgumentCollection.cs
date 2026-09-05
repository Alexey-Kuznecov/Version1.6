
using System.Collections.Generic;

namespace UnityCommander.Commands.Parsing
{
    public interface IArgumentCollection
    {
        bool HasFlag(string name);

        string? GetString(string name);

        IReadOnlyList<string> GetValues(string name);

        bool TryGetKeyValues(
          string name,
          out IReadOnlyList<KeyValuePair<string, object>> values);

        string? GetAt(int index);

        int GetInt(string name, int defaultValue = 0);
    }
}
