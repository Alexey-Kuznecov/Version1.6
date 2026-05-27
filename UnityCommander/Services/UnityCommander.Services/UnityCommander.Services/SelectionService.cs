
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Selection
{
    public class SelectionService : ISelectionService
    {
        readonly Dictionary<string, WeakReference<ISelectionManager>> _map = new();

        readonly object _lock = new();

        private readonly ITabContextAccessor _tabContextAccessor;

        public SelectionService(ITabContextAccessor tabContextAccessor)
        {
            _tabContextAccessor = tabContextAccessor;
        }

        public void Register(string panelId, ISelectionManager manager)
        {
            lock (_lock)
            {
                _map[panelId] = new WeakReference<ISelectionManager>(manager);
            }
        }

        public void Unregister(string panelId)
        {
            lock (_lock)
            {
                _map.Remove(panelId);
            }
        }

        public ISelectionManager Get(string panelId)
        {
            lock (_lock)
            {
                if (_map.TryGetValue(panelId, out var wr) && wr.TryGetTarget(out var mgr))
                    return mgr;
                _map.Remove(panelId);
                return null;
            }
        }

        public ISelectionManager GetActive()
            => Get(_tabContextAccessor.ActiveTabId);
    }
}
