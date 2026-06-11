using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class TabRegistry : ITabRegistry, IDiagnosticSource
    {
        private readonly Dictionary<Guid, ITabContentAdapter> _map = new();
        
        private readonly object _lock = new();

        private Guid? _activeTabId;
        
        private ITabContentAdapter _activeTab;

        /// <summary>
        /// Событие вызывается при смене активной панели.
        /// </summary>
        public event Action<ITabContentAdapter> ActiveTabChanged;

        /// <summary>
        /// Текущая активная панель (или null, если не установлена)
        /// </summary>
        public ITabContentAdapter ActiveTab
        {
            get => _activeTab;
            private set
            {
                if (!ReferenceEquals(_activeTab, value))
                {
                    _activeTab = value;
                    ActiveTabChanged?.Invoke(_activeTab);
                }
            }
        }

        /// <summary>
        /// Регистрация новой панели.
        /// </summary>
        public void Register(ITabContentAdapter adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            lock (_lock)
            {
                _map[adapter.TabId] = adapter;

                if (_activeTabId == null)
                {
                    _activeTabId = adapter.TabId;
                    ActiveTab = adapter;
                }
            }
        }

        /// <summary>
        /// Удаление панели из реестра.
        /// </summary>
        public void Unregister(Guid tabId)
        {
            lock (_lock)
            {
                if (!_map.Remove(tabId, out var adapter))
                    return;

                adapter.Dispose(); // 👈 вот тут вся чистка

                if (_activeTabId == tabId)
                {
                    _activeTabId = Guid.Empty;
                    ActiveTab = null;

                    // 👇 восстановление состояния
                    var next = _map.Values.FirstOrDefault();
                    if (next != null)
                    {
                        _activeTabId = next.TabId;
                        ActiveTab = next;
                    }
                }
            }
        }

        /// <summary>
        /// Получить панель по ID.
        /// </summary>
        public ITabContentAdapter GetTab(Guid tabId)
        {
            lock (_lock)
            {
                _map.TryGetValue(tabId, out var adapter);
                return adapter;
            }
        }

        /// <summary>
        /// Получить все панели.
        /// </summary>
        public IReadOnlyList<ITabContentAdapter> GetAllTabs()
        {
            lock (_lock)
            {
                return _map.Values.ToList();
            }
        }

        /// <summary>
        /// Получить активную панель. Если активная не выбрана — ошибка.
        /// </summary>
        public void SetActive(Guid tabId)
        {
            lock (_lock)
            {
                if (!_map.TryGetValue(tabId, out var adapter))
                    throw new InvalidOperationException($"Tab '{tabId}' is not registered.");

                _activeTabId = tabId;
                ActiveTab = adapter;
            }
        }

        /// <summary>
        /// Установить активную панель.
        /// </summary>
        public ITabContentAdapter GetActiveTab()
        {
            lock (_lock)
            {
                if (_activeTabId == null || !_map.TryGetValue(_activeTabId.Value, out var adapter))
                    throw new InvalidOperationException("Active tab is not set.");

                return adapter;
            }
        }

        public bool Contains(Guid tabId)
            => _map.ContainsKey(tabId);

        public string Name => "tab";

        public object GetState()
        {
            return new Dictionary<string, object>
            {
                ["TabCount"] = _map.Count,
                ["ActiveTabId"] = _activeTabId,
                ["Path"] = _activeTab.GetCurrentPath()
            };
        }

        public string Describe()
        {
            return new string(
                "Tab registry diagnostics.\n" +
                $"Tab count: {_map.Count}\n" +
                $"Active tab: {_activeTabId}\n");
        }
    }
}
