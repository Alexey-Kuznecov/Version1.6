
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Statusbar;
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Core.StatusBar
{
    public class StatusBarRegistry : IStatusBarRegistry
    {
        private readonly Dictionary<string, IStatusBarItem> _items = new();

        public event Action<string>? OwnerUnload;

        public void Register(IStatusBarItem statusBarItem)
        {
            ArgumentNullException.ThrowIfNull(statusBarItem);

            if (!_items.TryAdd(statusBarItem.Id, statusBarItem))
                throw new InvalidOperationException(
                    $"Status bar item '{statusBarItem.Id}' is already registered.");
        }

        public void Unregister(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            _items.Remove(id);
        }

        public IStatusBarItem Get(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            if (!_items.TryGetValue(id, out var item))
                throw new KeyNotFoundException(
                    $"Status bar item '{id}' is not registered.");

            return item;
        }

        public IEnumerable<IStatusBarItem> GetAll()
        {
            return _items.Values;
        }

        public void Cleanup(string ownerId)
        {
            ArgumentException.ThrowIfNullOrEmpty(ownerId);

            var ids = _items.Values
                .Where(x => x.OwnerId == ownerId)
                .Select(x => x.Id)
                .ToArray();

            foreach (var id in ids)
            {
                _items.Remove(id);
            }

            OwnerUnload?.Invoke(ownerId);
        }
    }
}
