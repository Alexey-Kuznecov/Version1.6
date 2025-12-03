using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class PanelRegistry : IPanelRegistry
    {
        private readonly Dictionary<string, IPanelContentProvider> _map = new();
        private readonly object _lock = new();

        private string? _activePanelId;
        private IPanelContentProvider? _activePanel;

        /// <summary>
        /// Событие вызывается при смене активной панели.
        /// </summary>
        public event Action<IPanelContentProvider?>? ActivePanelChanged;

        /// <summary>
        /// Текущая активная панель (или null, если не установлена)
        /// </summary>
        public IPanelContentProvider ActivePanel
        {
            get => _activePanel;
            private set
            {
                if (!ReferenceEquals(_activePanel, value))
                {
                    _activePanel = value;
                    ActivePanelChanged?.Invoke(_activePanel);
                }
            }
        }

        /// <summary>
        /// Регистрация новой панели.
        /// </summary>
        public void RegisterPanel(IPanelContentProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            lock (_lock)
            {
                _map[provider.PanelId] = provider;

                // Если активная панель еще не выбрана — выбираем первую добавленную
                if (_activePanelId == null)
                {
                    _activePanelId = provider.PanelId;
                    ActivePanel = provider;
                }
            }
        }

        /// <summary>
        /// Удаление панели из реестра.
        /// </summary>
        public void UnregisterPanel(string panelId)
        {
            lock (_lock)
            {
                if (_map.Remove(panelId))
                {
                    // если удалили активную панель → выбираем любую другую
                    if (_activePanelId == panelId)
                    {
                        _activePanelId = _map.Keys.FirstOrDefault();
                        ActivePanel = _activePanelId != null ? _map[_activePanelId] : null;
                    }
                }
            }
        }

        /// <summary>
        /// Получить панель по ID.
        /// </summary>
        public IPanelContentProvider? GetPanel(string panelId)
        {
            lock (_lock)
            {
                _map.TryGetValue(panelId, out var provider);
                return provider;
            }
        }

        /// <summary>
        /// Получить все панели.
        /// </summary>
        public IReadOnlyList<IPanelContentProvider> GetAllPanels()
        {
            lock (_lock)
            {
                return _map.Values.ToList();
            }
        }

        /// <summary>
        /// Получить активную панель. Если активная не выбрана — ошибка.
        /// </summary>
        public IPanelContentProvider GetActivePanel()
        {
            lock (_lock)
            {
                if (_activePanelId == null || !_map.TryGetValue(_activePanelId, out var provider))
                    throw new InvalidOperationException("Active panel is not set or not available.");

                return provider;
            }
        }

        /// <summary>
        /// Установить активную панель.
        /// </summary>
        public void SetActivePanel(string panelId)
        {
            lock (_lock)
            {
                if (!_map.TryGetValue(panelId, out var provider))
                    throw new InvalidOperationException($"Panel '{panelId}' is not registered.");

                _activePanelId = panelId;
                ActivePanel = provider;
            }
        }
    }
}
