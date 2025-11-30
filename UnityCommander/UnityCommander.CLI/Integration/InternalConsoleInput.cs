
using System.Collections.Concurrent;
using UnityCommander.CLI.Core;

namespace UnityCommander.CLI.Integration
{
    public class InternalConsoleInput : IConsoleInput
    {
        private readonly BlockingCollection<string> _queue = new();

        public void Submit(string line)
        {
            if (!string.IsNullOrWhiteSpace(line))
                _queue.Add(line);
        }

        public Task<string?> ReadLineAsync(CancellationToken token)
            => Task.Run(() => _queue.Take(token), token);
    }
}
