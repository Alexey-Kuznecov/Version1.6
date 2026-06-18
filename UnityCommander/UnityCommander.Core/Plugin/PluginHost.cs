
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Core.Plugin
{
    public sealed class PluginHost
    {
        private readonly Dictionary<string, PluginInstance> _plugins = new();
        private readonly object _lock = new();

        public IReadOnlyCollection<PluginInstance> Plugins
        {
            get
            {
                lock (_lock)
                    return _plugins.Values.ToList();
            }
        }

        // регистрация при активации (веб, кнопка, загрузчик — не важно)
        public void Register(PluginInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            lock (_lock)
            {
                if (_plugins.ContainsKey(instance.PluginId))
                    throw new InvalidOperationException(
                        $"Plugin '{instance.PluginId}' already registered.");

                _plugins[instance.PluginId] = instance;
            }
        }

        // получение
        public PluginInstance Get(string pluginId)
        {
            if (pluginId == null)
                throw new ArgumentNullException(nameof(pluginId));

            lock (_lock)
            {
                if (!_plugins.TryGetValue(pluginId, out var plugin))
                    throw new KeyNotFoundException(
                        $"Plugin '{pluginId}' not found.");

                return plugin;
            }
        }

        public bool TryGet(string pluginId, out PluginInstance instance)
        {
            lock (_lock)
                return _plugins.TryGetValue(pluginId, out instance);
        }

        // проверка
        public bool IsLoaded(string pluginId)
        {
            lock (_lock)
                return _plugins.ContainsKey(pluginId);
        }

        // выгрузка
        public bool Unload(string pluginId)
        {
            PluginInstance? instance;

            lock (_lock)
            {
                if (!_plugins.TryGetValue(pluginId, out instance))
                    return false;

                _plugins.Remove(pluginId);
            }

            try
            {
                instance.Dispose(); // если есть
            }
            catch
            {
                // логируешь, но не валишь систему
            }

            return true;
        }

        // массовая очистка
        public void Clear()
        {
            List<PluginInstance> copy;

            lock (_lock)
            {
                copy = _plugins.Values.ToList();
                _plugins.Clear();
            }

            foreach (var plugin in copy)
            {
                try { plugin.Dispose(); }
                catch { /* ignore */ }
            }
        }
    }
}
