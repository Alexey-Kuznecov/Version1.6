
namespace UnityCommander.Services.Plugins.NETCORE3_1
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

#if NETCOREAPP3_1
    public class PluginLoader
    {
        private PluginConfig config;
        private AssemblyLoadBuilder loadBuilder;
        private WeakReference weak;

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

            this.weak = this.loadBuilder.Build();

            if (this.weak.Target is PluginLoadContext loadContext)
            {
                loadContext.Unloading += LoadContext_Unloading;
                loadContext.Resolving += LoadContext_Resolving;
            }
        }

        private Assembly LoadContext_Resolving(System.Runtime.Loader.AssemblyLoadContext arg1, AssemblyName arg2)
        {
            return null;
        }

        private void LoadContext_Unloading(System.Runtime.Loader.AssemblyLoadContext obj)
        {
        }

        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, Type[] sharedTypes)
        {
            var config = new PluginConfig(assemblyFile);

            //if (sharedTypes != null)
            //{
            //    var uniqueAssemblies = new HashSet<Assembly>();
            //    foreach (var type in sharedTypes)
            //    {
            //        uniqueAssemblies.Add(type.Assembly);
            //    }

            //    foreach (var assembly in uniqueAssemblies)
            //    {
            //        config.SharedAssemblies.Add(assembly.GetName());
            //    }
            //}

            return new PluginLoader(config);
        }

        public Assembly LoadDefaultAssembly()
        {
            if (this.weak.Target is PluginLoadContext loadContext) return loadContext.LoadFromAssemblyPath(this.config.MainAssemblyPath);
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PluginUnload()
        {
            if (this.weak.Target is PluginLoadContext loadContext)
            {
                loadContext.PluginUnload();
                this.loadBuilder.PluginUnload();
               // this.config.PluginUnload();
                loadContext.Unload();
                loadContext = null;
               // this.config = null;
                loadContext = null;
                this.loadBuilder = null;
            };

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                if (assembly.IsCollectible)
                {
                    var sda = assembly.FullName;
                }
            }

            //this.weak.Target = null;
            //this.weak = null;
#if DEBUG
            WeakReference reference = new WeakReference(config);
            WeakReference reference2 = new WeakReference(loadBuilder);

            Helper.WeekRefMethod.AddWatcher(this.weak);
            Helper.WeekRefMethod.AddWatcher(reference);
            Helper.WeekRefMethod.AddWatcher(reference2);
            Helper.WeekRefMethod.WeekRef();
#endif
        }
    }
#endif
        }
