
using System.Collections.Generic;

namespace UnityCommander.Common.Diagnostic
{
    public sealed class DiagnosticDefinition
    {
        public required string Name { get; init; }

        public required DiagnosticCardinality Cardinality { get; init; }

        public List<DiagnosticRegistration> Instances { get; } = new();
    }
}
