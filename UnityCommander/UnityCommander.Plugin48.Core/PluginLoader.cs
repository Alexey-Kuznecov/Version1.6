using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace UnityCommander.Plugin48.Core
{
    using System.IO;
    using System.Reflection;

    using UnityCommander.Plugin.Core;
    using UnityCommander.Plugin48.Core.Loader;

    public class PluginLoader
    {
        private readonly PluginConfig _config;
        private ManagedLoadContext _context;
        private readonly AssemblyLoadContextBuilder _contextBuilder;
        private volatile bool _disposed;

        /// <summary>
        /// Initialize an instance of <see cref="PluginLoader" />
        /// </summary>
        /// <param name="config">The configuration for the plugin.</param>
        public PluginLoader(PluginConfig config)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._contextBuilder = CreateLoadContextBuilder(config);
            this._context = (ManagedLoadContext)this._contextBuilder.Build();
#if FEATURE_UNLOAD
            if (config.EnableHotReload)
            {
                StartFileWatcher();
            }
#endif
        }

        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, Type[] sharedTypes)
        {
            return CreateFromAssemblyFile(assemblyFile, sharedTypes, _ => { });
        }

        public static PluginLoader CreateFromAssemblyFile(
            string assemblyFile,
            Type[] sharedTypes,
            Action<PluginConfig> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var config = new PluginConfig(assemblyFile);
            configure(config);
            return new PluginLoader(config);
        }

        public Assembly LoadDefaultAssembly()
        {
            this.EnsureNotDisposed();
            return this._context.LoadAssemblyFromFilePath(this._config.MainAssemblyPath);
        }

        private void EnsureNotDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(nameof(PluginLoader));
            }
        }

        private static AssemblyLoadContextBuilder CreateLoadContextBuilder(PluginConfig config)
        {
            var builder = new AssemblyLoadContextBuilder();

            builder.SetMainAssemblyPath(config.MainAssemblyPath);
            builder.SetDefaultContext(config.DefaultContext);

            foreach (var ext in config.PrivateAssemblies)
            {
                builder.PreferLoadContextAssembly(ext);
            }

            if (config.PreferSharedTypes)
            {
                builder.PreferDefaultLoadContext(true);
            }

#if FEATURE_UNLOAD
            if (config.IsUnloadable || config.EnableHotReload)
            {
                builder.EnableUnloading();
            }

            if (config.LoadInMemory)
            {
                builder.PreloadAssembliesIntoMemory();
                builder.ShadowCopyNativeLibraries();
            }
#endif

            builder.IsLazyLoaded(config.IsLazyLoaded);
            foreach (var assemblyName in config.SharedAssemblies)
            {
                builder.PreferDefaultLoadContextAssembly(assemblyName);
            }

#if !FEATURE_NATIVE_RESOLVER

            // In .NET Core 3.0, this code is unnecessary because the API, AssemblyDependencyResolver, handles parsing these files.
            var baseDir = Path.GetDirectoryName(config.MainAssemblyPath);
            var assemblyFileName = Path.GetFileNameWithoutExtension(config.MainAssemblyPath);

            var depsJsonFile = Path.Combine(baseDir, assemblyFileName + ".deps.json");
            if (File.Exists(depsJsonFile))
            {
                builder.AddDependencyContext(depsJsonFile);
            }

            var pluginRuntimeConfigFile = Path.Combine(baseDir, assemblyFileName + ".runtimeconfig.json");

            builder.TryAddAdditionalProbingPathFromRuntimeConfig(pluginRuntimeConfigFile, includeDevConfig: true, out _);

            // Always include runtimeconfig.json from the host app.
            // in some cases, like `dotnet test`, the entry assembly does not actually match with the
            // runtime config file which is why we search for all files matching this extensions.
            foreach (var runtimeconfig in Directory.GetFiles(AppContext.BaseDirectory, "*.runtimeconfig.json"))
            {
                builder.TryAddAdditionalProbingPathFromRuntimeConfig(runtimeconfig, includeDevConfig: true, out _);
            }
#endif

            return builder;
        }
    }
}
