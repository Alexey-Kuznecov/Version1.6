
using System.Collections.Generic;
using UnityCommander.Abstractions.History;

namespace UnityCommander.Common.History
{
    public class UserFavorites : IUserFavorites
    {
        private readonly List<string> _paths = [];

        public IReadOnlyList<string> Paths => _paths;

        public UserFavorites(IUserFavoriteStore favoriteStore)
        {
            foreach (var path in favoriteStore.Load())
            {
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                if (_paths.Contains(path))
                    continue;

                _paths.Add(path);
            }
        }

        public void Add(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (_paths.Contains(path))
                return;

            _paths.Add(path);
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
