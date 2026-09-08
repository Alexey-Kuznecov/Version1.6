
using System.Collections.Generic;
using UnityCommander.Abstractions.History;

namespace UnityCommander.Common.History
{
    public sealed class UserNavigationHistory : IUserNavigationHistory
    {
        private const int MaxItems = 20;

        private readonly List<string> _paths = [];

        public IReadOnlyList<string> Paths => _paths;

        public UserNavigationHistory(
            IUserNavigationHistoryStore store)
        {
            foreach (var path in store.Load())
            {
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                if (_paths.Contains(path))
                    continue;

                _paths.Add(path);

                if (_paths.Count >= MaxItems)
                    break;
            }
        }

        public void Add(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            _paths.Remove(path);
            _paths.Insert(0, path);

            if (_paths.Count > MaxItems)
                _paths.RemoveAt(_paths.Count - 1);
        }

        public void Remove(string path)
        {
            _paths.Remove(path);
        }

        public void Clear()
        {
            _paths.Clear();
        }
    }
}
