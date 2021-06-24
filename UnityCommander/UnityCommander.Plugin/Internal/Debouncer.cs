// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace UnityCommander.Plugin.Internal
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Debouncer : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly TimeSpan _waitTime;
        private int _counter;

        public Debouncer(TimeSpan waitTime)
        {
            this._waitTime = waitTime;
        }

        public void Execute(Action action)
        {
            var current = Interlocked.Increment(ref this._counter);

            Task.Delay(this._waitTime).ContinueWith(task =>
            {
                // Is this the last task that was queued?
                if (current == this._counter && !this._cts.IsCancellationRequested)
                {
                    action();
                }

                task.Dispose();
            }, this._cts.Token);
        }

        public void Dispose()
        {
            this._cts.Cancel();
        }
    }
}
