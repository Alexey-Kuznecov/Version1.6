

using System.Collections.Generic;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Core.Diagnostics
{
    public sealed class DiagnosticRegistry : IDiagnosticRegistry
    {
        private readonly Dictionary<string, IDiagnosticSource> _sources = new();

        public void Register(IDiagnosticSource source)
        {
            _sources[source.Name] = source;
        }

        public bool TryGet(string name, out IDiagnosticSource? source)
        {
            return _sources.TryGetValue(name, out source);
        }

        public IEnumerable<IDiagnosticSource> GetAll()
        {
            return _sources.Values;
        }

        public IDiagnosticSource Get(string name)
        {
            return _sources.TryGetValue(name, out var source)
                ? source
                : throw new KeyNotFoundException($"Diagnostic source '{name}' not found.");
        }
    }
}
