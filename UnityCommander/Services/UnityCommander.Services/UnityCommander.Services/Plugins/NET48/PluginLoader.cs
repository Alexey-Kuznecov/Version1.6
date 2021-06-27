using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityCommander.Services.Plugins.NET48
{
#if NET472
    public class PluginLoader
    {
        private readonly PluginConfig config;
        private readonly AssemblyLoadBuilder loadBuilder;
        private readonly PluginLoadContext context;

        public PluginLoader(PluginConfig config)
        {
            this.config = config;
            var builder = new AssemblyLoadBuilder();

            builder.SetMainAssemblyPath(config.MainAssemblyPath);
            builder.SetDefaultContext(config.DefaultContext);

            foreach (var assemblyName in config.SharedAssemblies)
            {
                builder.PreferDefaultLoadContextAssembly(assemblyName);
            }

            loadBuilder = builder;

            this.context = (PluginLoadContext)this.loadBuilder.Build();
        }

        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, Type[] sharedTypes)
        {
            var config = new PluginConfig(assemblyFile);
            
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
           
            return new PluginLoader(config);
        }

        public Assembly LoadDefaultAssembly()
        {
            return null;
        }

        public void PluginUnload()
        {
            //context.Unload();
        }
    }
#endif
}
