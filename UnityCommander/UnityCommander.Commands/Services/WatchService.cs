
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Commands.Diagnostic;

namespace UnityCommander.Commands.Services
{
    public class WatchService
    {
        private readonly IDiagnosticPipeline _pipeline;

        public WatchService(IDiagnosticPipeline diagnostic)
        {
            _pipeline = diagnostic;
        }

        public async Task Run(
            DiagnosticQuery query,
            int interval,
            Action<DiagnosticResult> onResult,
            CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var result = _pipeline.Execute(query);

                onResult(result);

                await Task.Delay(interval, ct);
            }
        }
    }
}
