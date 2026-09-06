
namespace UnityCommander.CLI.History
{
    public sealed class ConsoleHistoryService 
    {
        private readonly IConsoleHistory _history;
        private readonly IConsoleHistoryStore _store;

        public ConsoleHistoryService(
            IConsoleHistory history,
            IConsoleHistoryStore store)
        {
            _history = history;
            _store = store;
        }

        public void Initialize()
        {
            var commands = _store.Load();

            foreach (var command in commands)
                _history.Add(command);

            _history.Reset();
        }

        public void Save()
        {
            _store.Save(_history.Items);
        }
    }
}
