using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Plugins.Logging;

namespace UnityCommander.Plugins
{
    internal class PluginManager
    {
        private readonly PluginContainer _container = new();
        private readonly PluginLoader _loader = new();
        private readonly ILoggerService _logger;

        public PluginContainer Container => _container;

        public PluginManager(ILoggerService logger)
        {
            _logger = logger;
            _logger.Info("PluginManager initialized.");
        }

        public bool LoadPlugin(string path, IPluginContext context)
        {
            var factory = _loader.LoadPlugin(path);
            if (factory == null) return false;

            var plugin = factory.CreatePlugin();
            plugin.Initialize(context);
            _container.AddPlugin(plugin);
            return true;
        }

        public void UnloadAll()
        {
            foreach (var plugin in _container.Plugins)
            {
                plugin.Shutdown();
            }
            _container.Clear();
        }
    }
}
