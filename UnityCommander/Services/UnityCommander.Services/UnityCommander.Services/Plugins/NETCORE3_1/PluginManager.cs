using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Integration.Contracts;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Plugins.NETCORE3_1
{
#if NETCOREAPP3_1

#else
    using UnityCommander.Services.Plugins.NET48;
#endif

    public class PluginManager : IPluginManager
    {
        private WeakReference loader;
        
        private static List<WeakReference> weaks = new List<WeakReference>();

        /// <summary>
        /// Gets  the imported plugin implementations.
        /// </summary>
        private IEnumerable<IPluginImplement> pluginImplements;

        public PluginManager(string path)
        {
            var service = new ServiceCollection();
            var loaders = LoadPlugin(path);
            ConfigureServices(service, loaders);
            using var serviceProvider = service.BuildServiceProvider();
            
            pluginImplements = serviceProvider.GetServices<IPluginImplement>();
        }

        public List<WeakReference> LoadPlugin(string relativePath)
        {
            var loaders = new List<WeakReference>();

            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))))) ?? string.Empty));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));

            if (File.Exists(pluginLocation))
            {
                loader = new WeakReference(PluginLoader.CreateFromAssemblyFile(
                    pluginLocation,
                    sharedTypes: new[] {typeof(IPluginFactory), typeof(IServiceCollection)}));
                loaders.Add(loader);
            }

            return loaders;
        }

        public IEnumerable<IPluginImplement> GetPluginImplement()
        {
            foreach (var item in pluginImplements)
            {
                yield return item;
            }

            yield return null;
        }

        public IPluginFactory GetPluginFactory(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPluginFactory).IsAssignableFrom(type))
                {
                    IPluginFactory result = Activator.CreateInstance(type) as IPluginFactory;
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Configure plugins in a dependency injection collection.
        /// </summary>
        /// <param name="services"> Service collection. </param>
        /// <param name="loaders"> List of plugins loaded. </param>
        private void ConfigureServices(ServiceCollection services, List<WeakReference> loaders)
        {
            // Create an instance of plugin types
            foreach (var reference in loaders)
            {
                if (reference.Target is PluginLoader pluginLoader)
                    
                    foreach (var pluginType in pluginLoader
                        .LoadDefaultAssembly()
                        .GetTypes()
                        .Where(t => typeof(IPluginFactory).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        // This assumes the implementation of IPluginFactory has a parameter less constructor
                        var plugin = Activator.CreateInstance(pluginType) as IPluginFactory;

                        plugin?.Configure(services);
                        //PluginResourceManager.GetResourceDictionary(pluginType.Assembly);
                    }
            }
        }
        
        public void PluginUnload()
        {
#if FEATURE_UNLOAD
            foreach (var weak in weaks)
            {
                weak.Target = null;
            }

            pluginImplements = null;
            if (loader.Target is PluginLoader referenceLoader)
            {
                referenceLoader.PluginUnload();
            }
#endif
        }
    }
}
