
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using Prism.Dialogs;
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
            _manager.Prepare(pluginId);

            var container = _manager.GetContainerById(pluginId);
            
            if (container.IsActivated)
                return;

            //_resources.Load(container);
            GetPluginResources(container.LoadedAssembly);

            var plugin = (IPlugin)Activator.CreateInstance(container.PluginType);

            var registrar = new PluginRegistrar(container.PluginID);
            
            using (var init = new PluginInitContext(registrar))
            {
                plugin.Initialize(init);
            }

            registrar.Apply(_serviceProvider);

            var pluginProvider =
              BuildPluginProvider(
                  registrar,
                  _serviceProvider);

            var pluginService = new PluginServices(pluginProvider);
            var context = new PluginContext(pluginService, container.PluginID);

            container.Context = context;
            container.Services = pluginProvider;

            container.Activate(plugin);
        }

        private IServiceProvider BuildPluginProvider(
            PluginRegistrar registrar,
            IServiceProvider rootProvider)
        {
            var services = new ServiceCollection();

            RegisterHostServices(services, rootProvider);

            foreach (var descriptor in registrar.Services)
            {
                services.Add(descriptor);
            }

            return services.BuildServiceProvider();
        }

        private void RegisterHostServices(
            IServiceCollection services,
            IServiceProvider rootProvider)
        {
            services.AddSingleton(
                rootProvider.GetRequiredService<LoggerCreator>());

            services.AddSingleton(
                rootProvider.GetRequiredService<IDialogService>());
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
