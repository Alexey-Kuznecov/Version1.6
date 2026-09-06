
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.CLI.Lifecicle
{
    public class CommandProcessManager // : ICommandProcessManager
    {
        private class ProcessEntry
        {
            public required Guid Id { get; init; }
            public required string Name { get; init; }   // 👈 важно
            public required CancellationTokenSource Cts { get; init; }
            public required Task Task { get; init; }
        }

        private readonly object _lock = new();

        private readonly Dictionary<Guid, ProcessEntry> _processes = new();

        private readonly  ILogger _logger;

        public CommandProcessManager(LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<CommandProcessManager>(LogScope.Runtime);
        }

        public Guid Start(string name, Func<CancellationToken, Task> taskFactory)
        {
            var id = Guid.NewGuid();
            var cts = new CancellationTokenSource();

            var task = Task.Run(async () =>
            {
                try
                {
                    await taskFactory(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    _logger.Info($"Process {id} canceled");
                }
                catch (Exception ex)
                {
                    // тут можешь логировать
                    _logger.Error($"Process {id} crashed: {ex}");
                }
            }, CancellationToken.None);

            lock (_lock)
            {
                _processes[id] = new ProcessEntry
                {
                    Id = id,
                    Name = name,
                    Cts = cts,
                    Task = task
                };
            }

            return id;
        }

        public void Stop(Guid id)
        {
            ProcessEntry? entry;

            lock (_lock)
            {
                if (!_processes.TryGetValue(id, out entry))
                    return;

                _processes.Remove(id);
            }

            entry.Cts.Cancel();
        }

        public void StopByName(string name)
        {
            List<ProcessEntry> entries;

            lock (_lock)
            {
                entries = _processes.Values
                    .Where(p => p.Name == name)
                    .ToList();
            }

            foreach (var e in entries)
            {
                Stop(e.Id);
            }
        }

        public void StopAll()
        {
            List<ProcessEntry> entries;

            lock (_lock)
            {
                entries = _processes.Values.ToList();
                _processes.Clear();
            }

            foreach (var e in entries)
            {
                e.Cts.Cancel();
            }
        }
    }
}
