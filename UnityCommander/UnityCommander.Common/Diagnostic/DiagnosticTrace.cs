
using System.Collections.Generic;

namespace UnityCommander.Common.Diagnostic
{
    public sealed class DiagnosticTrace
    {
        private readonly Dictionary<object, string> _previous = new();

        public bool HasChanged(object key, string value)
        {
            if (!_previous.TryGetValue(key, out var previous))
            {
                _previous[key] = value;
                return false;
            }

            if (previous == value)
                return false;

            _previous[key] = value;
            return true;
        }
    }
}
