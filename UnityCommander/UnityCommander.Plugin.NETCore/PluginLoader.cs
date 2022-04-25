using System;
using System.Collections.Generic;
using System.Reflection;
#if !NET472
using System.Runtime.Loader;
#endif

namespace UnityCommander.Plugin.NETCore
{
    public class PluginLoader
    {
        private readonly PluginConfig config;
        private readonly AssemblyLoadContextBuilder contextBuilder;
        private readonly PluginLoadContext context;

        public AssemblyLoadContext LoadContext { get; set; }

        public PluginLoader(PluginConfig config)
        {
            this.config = config;
            this.contextBuilder = CreateLoadContextBuilder(config);
            this.context = (PluginLoadContext)this.contextBuilder.Build();
        }

        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, Type[] sharedTypes, Action<PluginConfig> configure)
        {
            return CreateFromAssemblyFile(assemblyFile,
                config =>
                {
                    if (sharedTypes != null)
                    {
                        var uniqueAssemblies = new HashSet<Assembly>();
                        foreach (var type in sharedTypes)
                        {
                            uniqueAssemblies.Add(type.Assembly);
                        }

                        foreach (var assembly in uniqueAssemblies)
                        {
                            config.SharedAssemblies.Add(assembly.GetName());
                        }
                    }
                    configure(config);
                });
        }

        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, Action<PluginConfig> configure)
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
            return this.context.LoadAssemblyFromFilePath(this.config.MainAssemblyPath);
        }

        private static AssemblyLoadContextBuilder CreateLoadContextBuilder(PluginConfig config)
        {
            var builder = new AssemblyLoadContextBuilder();

            builder.SetMainAssemblyPath(config.MainAssemblyPath);
            builder.SetDefaultContext(config.DefaultContext);

            builder.IsLazyLoaded(config.IsLazyLoaded);

            foreach (var assemblyName in config.SharedAssemblies)
            {
                builder.PreferDefaultLoadContextAssembly(assemblyName);
            }

            return builder;
        }
    }
}
