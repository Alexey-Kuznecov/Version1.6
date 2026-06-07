
using System.Collections.Generic;

namespace UnityCommander.Commands.Diagnostic
{
    public sealed class DiagnosticQuery
    {
        public string Source { get; init; }

        public string Format { get; init; } = "table";

        public Dictionary<string, string>? Filters { get; init; }
    }
}
