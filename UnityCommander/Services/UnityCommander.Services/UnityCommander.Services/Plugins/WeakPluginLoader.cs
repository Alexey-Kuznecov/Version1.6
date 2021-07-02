using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Dialog;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Plugins
{
    public class WeakPluginLoader : IPluginLoader
    {
        private Guid pluginToken = Guid.NewGuid();
        private IEnumerable<IPluginImplement> pluginImplements;
        private IEnumerable<IPluginConfigure> pluginSettings;
        private IEnumerable<IDialogService> dialogService;
        private IEnumerable<IPluginDescriptor> pluginMeta;
        private HostPluginLoadContext alc;
        private WeakReference weakReference;
        private HashSet<ResourceDictionary> pluginResources = new ();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ExecuteAndUnload(string assemblyPath)
        {
            alc = new HostPluginLoadContext(assemblyPath);
            var services = new ServiceCollection();
            weakReference = new WeakReference(alc);
            Assembly assembly = alc.LoadFromAssemblyPath(assemblyPath);

            this.GetPluginResources(assembly);

            foreach (var type in assembly.GetTypes()
                .Where(t => typeof(IPluginFactory)
                .IsAssignableFrom(t) && !t.IsAbstract))
            {
                var plugin = Activator.CreateInstance(type) as IPluginFactory;
                plugin?.Configure(services);

                var serviceProvider = services.BuildServiceProvider();
                pluginImplements = serviceProvider.GetServices<IPluginImplement>();
                pluginSettings = serviceProvider.GetServices<IPluginConfigure>();
                pluginMeta = serviceProvider.GetServices<IPluginDescriptor>();
                dialogService = serviceProvider.GetServices<IDialogService>();
            }
        }

        private void GetPluginResources(Assembly assembly)
        {
            pluginResources = PluginResourceManager.GetResourceDictionary(assembly);

            if (pluginResources?.Count is not 0 && pluginResources != null)
            {
                var dictionary = Application.Current.Resources.MergedDictionaries;

                foreach (var resource in pluginResources)
                {
                    dictionary.Add(resource);
                }
            }
        }
        
        public IEnumerable<IPluginConfigure> GetConfigurations()
        {
            return pluginSettings;
        }

        public IEnumerable<IPluginDescriptor> GetDescriptors()
        {
            return pluginMeta;
        }

        public IEnumerable<IDialogService> GetDialogs()
        {
            return dialogService;
        }

        public IEnumerable<IPluginImplement> GetImplements()
        {
            return pluginImplements;
        }

        public bool UnloadPlugin()
        {
            alc.Unload();
            pluginImplements = null;
            pluginSettings   = null;
            pluginMeta       = null;
            dialogService    = null;
            alc              = null;

            if (pluginResources?.Count > 0 && pluginResources != null)
            {
                foreach (var resource in pluginResources)
                {
                    var dictionary = Application.Current.Resources.MergedDictionaries;
                    dictionary.Remove(resource);
                }
            }

            pluginResources = null;

            for (int i = 0; weakReference.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Debug.WriteLine($"Unload success: {!weakReference.IsAlive}");
            return !weakReference.IsAlive;
        }
    }
}
