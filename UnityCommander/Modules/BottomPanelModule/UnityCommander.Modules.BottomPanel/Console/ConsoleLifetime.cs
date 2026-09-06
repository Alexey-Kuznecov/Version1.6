
using System;
using System.Threading;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleLifetime : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();

        public CancellationToken Token => _cts.Token;

        public bool IsRunning => !_cts.IsCancellationRequested;

        public void Stop()
        {
            if (!_cts.IsCancellationRequested)
                _cts.Cancel();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
