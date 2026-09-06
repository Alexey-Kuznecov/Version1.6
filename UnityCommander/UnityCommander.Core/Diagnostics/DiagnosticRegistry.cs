

using System;
using System.Collections.Generic;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Core.Diagnostics
{
    public sealed class DiagnosticRegistry : IDiagnosticRegistry
    {
        private readonly Dictionary<string, DiagnosticDefinition> _diagnostics = new();

        public void Register(IDiagnostic diagnostic)
        {
            var definition = GetOrCreate(
                diagnostic.Name,
                diagnostic.Cardinality);

            if (definition.Cardinality == DiagnosticCardinality.Single &&
                definition.Instances.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Diagnostic '{diagnostic.Name}' " +
                    "does not support multiple instances.");
            }

            definition.Instances.Add(
                new DiagnosticRegistration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Diagnostic = diagnostic
                });
        }

        public DiagnosticDefinition Get(string name)
        {
            return _diagnostics.TryGetValue(name, out var diagnostic)
                ? diagnostic
                : throw new KeyNotFoundException(
                    $"Diagnostic '{name}' not found.");
        }

        public bool TryGet(
            string name,
            out DiagnosticDefinition? diagnostic)
        {
            return _diagnostics.TryGetValue(name, out diagnostic);
        }

        public IEnumerable<DiagnosticDefinition> GetAll()
        {
            return _diagnostics.Values;
        }

        private DiagnosticDefinition GetOrCreate(
            string name,
            DiagnosticCardinality cardinality)
        {
            if (_diagnostics.TryGetValue(name, out var diagnostic))
            {
                if (diagnostic.Cardinality != cardinality)
                {
                    throw new InvalidOperationException(
                        $"Diagnostic '{name}' is already registered " +
                        $"with cardinality '{diagnostic.Cardinality}'.");
                }

                return diagnostic;
            }

            diagnostic = new DiagnosticDefinition
            {
                Name = name,
                Cardinality = cardinality
            };

            _diagnostics.Add(name, diagnostic);

            return diagnostic;
        }
    }
}
