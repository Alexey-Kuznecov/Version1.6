using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Dialog;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class WeakPluginLoader : IPluginLoader
    {
        private IEnumerable<IPluginImplement> pluginImplements;
        private IEnumerable<IPluginConfigure> pluginSettings;
        private IEnumerable<IDialogService> dialogService;
        private IEnumerable<IPluginDescriptor> pluginMeta;
        private HostPluginLoadContext alc;
        private WeakReference weakReference;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ExecuteAndUnload(string assemblyPath)
        {
            alc = new HostPluginLoadContext(assemblyPath);
            var services = new ServiceCollection();
            weakReference = new WeakReference(alc);
            Assembly a = alc.LoadFromAssemblyPath(assemblyPath);
            UnityCommander.Services.Plugins.ResourceManager.GetResourceDictionary(a);
            foreach (var type in a.GetTypes()
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
