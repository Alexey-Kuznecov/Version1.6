
namespace UnityCommander.Services.Plugins.NETCORE3_1
{
#if NETCOREAPP3_1
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Loader;

    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver resolver;
        private HashSet<string> defaultAssemblies;
        private AssemblyLoadContext defaultLoadContext;
        private HashSet<string> privateAssemblies;

        public PluginLoadContext(
            string pluginPath,
            AssemblyLoadContext defaultLoadContext,
            HashSet<string> defaultAssemblies) : base(true)
        {
            this.defaultLoadContext = defaultLoadContext;
            this.defaultAssemblies = defaultAssemblies;
            //resolver = new AssemblyDependencyResolver(pluginPath);
            defaultLoadContext.Unloading += DefaultLoadContext_Unloading;
        }

        private void DefaultLoadContext_Unloading(AssemblyLoadContext obj)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (this.defaultAssemblies.Contains(assemblyName.Name))
            {
                var assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath != null)
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }

        public Assembly LoadAssemblyFromFilePath(string path)
        {
            using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var pdbPath = Path.ChangeExtension(path, ".pdb");
            if (File.Exists(pdbPath))
            {
                using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return this.LoadFromStream(file, pdbFile);
            }

            return this.LoadFromStream(file);
        }

        public void PluginUnload()
        {
#if FEATURE_UNLOAD
            this.resolver = null;
            this.defaultAssemblies = null;
            this.defaultLoadContext = null;
#endif
        }
    }
#endif
}
