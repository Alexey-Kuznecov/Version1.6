
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Diagnostics.Diagnostic;

namespace UnityCommander.Commands.Services
{
    public class WatchService
    {
        public async Task Run(
            int interval,
            Action onResult,
            CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                onResult();

                await Task.Delay(interval, ct);
            }
        }
    }
}
