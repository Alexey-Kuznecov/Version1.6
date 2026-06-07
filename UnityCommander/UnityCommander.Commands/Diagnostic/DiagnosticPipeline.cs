
using System;
using System.Collections.Generic;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Commands.Diagnostic
{
    public class DiagnosticPipeline : IDiagnosticPipeline
    {
        private readonly IDiagnosticRegistry _registry;

        public DiagnosticPipeline(IDiagnosticRegistry registry)
        {
            _registry = registry;
        }

        public DiagnosticResult Execute(
            DiagnosticQuery query)
        {
            var source = _registry.Get(query.Source);

            var state = source.GetState();

            return new DiagnosticResult
            {
                Source = source.Name,
                Values = state
            };
        }

        private IReadOnlyDictionary<string, object> ApplyFilters(IReadOnlyDictionary<string, object> state, Dictionary<string, string> filters)
         {
             throw new NotImplementedException();
         }

        private IReadOnlyDictionary<string, object> ApplyFormat(IReadOnlyDictionary<string, object> state, string format)
         {
             throw new NotImplementedException();
         }
    }
}
