
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Plugin;
using UnityCommander.Core.Plugin;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces.Plugins;
using UnityCommander.WPF;

namespace UnityCommander.Services.Plugins
{
    public class PluginActivator : IPluginActivator
    {
        private HashSet<ResourceDictionary> pluginResources = new();

        private IPluginManager _manager;

        private IServiceProvider _serviceProvider;

        private PluginHost _host;

        public PluginActivator(
            IPluginManager manager, 
            IServiceProvider serviceProvider,
            PluginHost host)
        {
            _serviceProvider = serviceProvider;
            _manager = manager;
            _host = host;
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
            
            var instance = new PluginInstance(
                pluginId,
                pluginProvider, 
                context);

            _host.Register(instance);

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

            services.AddSingleton(
                rootProvider.GetRequiredService<IWindowManager>());
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
            this.pluginResources = PluginResourceManager.GetResourceDictionary(assembly);

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
