
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UnityCommander.Common.Selection;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Selection
{
    public class SelectionService : ISelectionService
    {
        readonly Dictionary<string, WeakReference<ISelectionManager>> _map = new();
        string? _activeId;
        readonly object _lock = new();

        public void Register(string panelId, ISelectionManager manager)
        {
            lock (_lock)
            {
                _map[panelId] = new WeakReference<ISelectionManager>(manager);
                _activeId ??= panelId; // если нет активного — назначим
            }
        }

        public void Unregister(string panelId)
        {
            lock (_lock)
            {
                _map.Remove(panelId);
                if (_activeId == panelId) _activeId = _map.Keys.FirstOrDefault();
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
            => _activeId == null ? null : Get(_activeId);

        public void SetActive(string panelId)
        {
            lock (_lock)
            {
                if (_map.ContainsKey(panelId)) _activeId = panelId;
            }
        }
    }
}
