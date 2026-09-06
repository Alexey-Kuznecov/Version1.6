
using UnityCommander.Common.Diagnostic;
using UnityCommander.Diagnostics.Diagnostic;

namespace UnityCommander.Diagnostics.Reporting
{
    public sealed class DiagnosticPipeline : IDiagnosticPipeline
    {
        private readonly IDiagnosticRegistry _registry;

        private DiagnosticTrace _trace;

        public DiagnosticPipeline(IDiagnosticRegistry registry)
        {
            _registry = registry;
            _trace = new DiagnosticTrace();
        }

        public DiagnosticResult Execute(DiagnosticQuery query)
        {
            var diagnostic = _registry.Get(query.Source);

            var source = diagnostic.Instances
                .Select(x => x.Diagnostic)
                .OfType<IDiagnosticSource>()
                .FirstOrDefault();

            if (source is null)
            {
                throw new InvalidOperationException(
                    $"Diagnostic '{query.Source}' does not provide a source.");
            }

            return new DiagnosticResult
            {
                Source = source.Name,
                Values = source.GetState()
            };
        }

        public void Report(
            DiagnosticQuery query,
            IDiagnosticWriter writer)
        {
            var diagnostic = _registry.Get(query.Source);

            foreach (var instance in diagnostic.Instances)
            {
                if (instance.Diagnostic is IDiagnosticReporter reporter)
                    reporter.Report(writer);
            }
        }

        public bool ReportChanged(
            DiagnosticQuery query,
            IDiagnosticWriter writer)
        {
            var diagnostic = _registry.Get(query.Source);

            var hasChanges = false;

            foreach (var instance in diagnostic.Instances)
            {
                if (instance.Diagnostic is not IDiagnosticReporter reporter)
                    continue;

                var instanceWriter = new DiagnosticBufferWriter();

                reporter.Report(instanceWriter);

                var text = instanceWriter.ToString();

                if (!_trace.HasChanged(instance, text))
                    continue;

                writer.WriteLine(text);
                hasChanges = true;
            }

            return hasChanges;
        }
    }
}
