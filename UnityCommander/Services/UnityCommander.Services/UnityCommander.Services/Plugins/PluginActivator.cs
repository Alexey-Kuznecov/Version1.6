
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander.Services.Plugins
{
    public class PluginActivator : IPluginActivator
    {
        private HashSet<ResourceDictionary> pluginResources = new();

        private readonly IPluginResourceManager _resources;

        private LoggerCreator _logger;

        private IPluginManager _manager;

        private IServiceProvider _serviceProvider;
       
        public PluginActivator(
            IPluginManager manager, 
            IServiceProvider serviceProvider,
            IPluginResourceManager resources,
            LoggerCreator logger)
        {
            _resources = resources;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _manager = manager;
        }

        public void Activate(string pluginId)
        {
            var container = _manager.GetContainerById(pluginId);
            
            if (container.IsActivated)
                return;

            //_resources.Load(container);
            GetPluginResources(container.LoadedAssembly);

            var logger = _logger.ForPlugin($"PluginActivator:{container.PluginID}");

            var plugin = (IPlugin)Activator.CreateInstance(container.PluginType);

            var registrar = new PluginRegistrar();
            var initContext = new PluginInitContext(registrar);

            plugin.Initialize(initContext);

            registrar.Apply(_serviceProvider);

            var context = new PluginContext(_serviceProvider, container.PluginID, logger);

            container.Context = context;

            container.Activate(plugin);
        }

        public void ActivateStartupPlugins()
        {
            foreach (var info in _manager.GetAllPluginInfo())
            {
                if (info.AutoRun)
                {
                    Activate(info.DeveloperID);
                }
            }
        }

        private void GetPluginResources(Assembly assembly)
        {
            // Получаем ресурсы из менеджера ресурсов плагинов
            this.pluginResources = Integration.Plugins.PluginResourceManager.GetResourceDictionary(assembly);

            // Если ресурсы присутствуют, добавляем их в глобальные ресурсы приложения
            if (this.pluginResources?.Count != 0 && this.pluginResources != null)
            {
                var dictionary = Application.Current.Resources.MergedDictionaries;

                foreach (var resource in this.pluginResources)
                {
                    dictionary.Add(resource);
                }
            }
        }
    }
}
