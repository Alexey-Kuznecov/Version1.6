
namespace UnityCommander.CLI.History
{
    public sealed class ConsoleHistory : IConsoleHistory
    {
        private readonly List<string> _items = new();

        public IReadOnlyList<string> Items => _items;

        private int _index;

        public void Add(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;

            _items.RemoveAll(x =>
                string.Equals(x, command, StringComparison.Ordinal));

            //if (_items.Count > 0 &&
            //    string.Equals(_items[^1], command, StringComparison.Ordinal))
            //{
            //    Reset();
            //    return;
            //}

            _items.Add(command);
            _index = _items.Count;
        }

        public string? Previous()
        {
            if (_items.Count == 0)
                return null;

            if (_index > 0)
                _index--;

            return _items[_index];
        }

        public string? Next()
        {
            if (_items.Count == 0)
                return null;

            if (_index < _items.Count - 1)
            {
                _index++;
                return _items[_index];
            }

            _index = _items.Count;
            return null;
        }

        public void Reset()
        {
            _index = _items.Count;
        }
    }
}
