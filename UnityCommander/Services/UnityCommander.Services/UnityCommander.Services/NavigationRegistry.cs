
using System;
using System.Collections.Generic;
using UnityCommander.Core.Navigation;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class NavigationRegistry : INavigationRegistry
    {
        private readonly Dictionary<Guid, NavigationManager> _items = [];

        public NavigationManager Get(Guid tabId)
        {
            if (!_items.TryGetValue(tabId, out var navigation))
                throw new KeyNotFoundException(
                    $"Navigation for tab '{tabId}' was not found.");

            return navigation;
        }

        public void Register(Guid tabId, NavigationManager navigation)
        {
            _items.Add(tabId, navigation);
        }

        public bool Remove(Guid tabId)
        {
            return _items.Remove(tabId);
        }
    }
}
