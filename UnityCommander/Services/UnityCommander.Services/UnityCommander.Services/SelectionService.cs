
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Selection
{
    public class SelectionService : ISelectionService
    {
        readonly Dictionary<Guid, WeakReference<ISelectionManager>> _map = new();

        readonly object _lock = new();

        private readonly ITabContextAccessor _tabContextAccessor;

        public SelectionService(ITabContextAccessor tabContextAccessor)
        {
            _tabContextAccessor = tabContextAccessor;
        }

        public void Register(Guid tabId, ISelectionManager manager)
        {
            lock (_lock)
            {
                _map[tabId] = new WeakReference<ISelectionManager>(manager);
            }
        }


        public void Unregister(Guid tabId)
        {
            lock (_lock)
            {
                _map.Remove(tabId);
            }
        }

        public ISelectionManager Get(Guid tabId)
        {
            lock (_lock)
            {
                if (_map.TryGetValue(tabId, out var wr) && wr.TryGetTarget(out var mgr))
                    return mgr;
                _map.Remove(tabId);
                return null;
            }
        }

        public ISelectionManager GetActive()
            => Get(_tabContextAccessor.ActiveTabId);
    }
}
