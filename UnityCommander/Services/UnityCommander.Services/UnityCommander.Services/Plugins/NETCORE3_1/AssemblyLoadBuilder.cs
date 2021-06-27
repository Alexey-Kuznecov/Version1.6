
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityCommander.Services.Plugins.NETCORE3_1
{
#if NETCOREAPP3_1
    using System.Runtime.Loader;
    internal class AssemblyLoadBuilder
    {
        private HashSet<string> privateAssemblies = new(StringComparer.Ordinal);
        private HashSet<string> defaultAssemblies = new(StringComparer.Ordinal);
        private string mainAssemblyPath;
        public AssemblyLoadContext defaultLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;

        public WeakReference Build()
        {
            var alcWeakRef = new WeakReference(new PluginLoadContext(
                this.mainAssemblyPath,
                this.defaultLoadContext,
                this.defaultAssemblies));
            return alcWeakRef;
        }

        public AssemblyLoadBuilder PreferDefaultLoadContextAssembly(AssemblyName assemblyName)
        {
            this.defaultAssemblies.Add(assemblyName.Name);
            var assembly = this.defaultLoadContext.LoadFromAssemblyName(assemblyName);
            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                if (reference.Name != null)
                {
                    this.defaultAssemblies.Add(reference.Name);
                }
            }

            return this;
        }

        public AssemblyLoadBuilder SetDefaultContext(AssemblyLoadContext context)
        {
            this.defaultLoadContext = context ?? throw new ArgumentException($"Bad Argument: AssemblyLoadContext in {nameof(AssemblyLoadBuilder)}.{nameof(this.SetDefaultContext)} is null.");
            return this;
        }

        public AssemblyLoadBuilder SetMainAssemblyPath(string path)
        {
            this.mainAssemblyPath = path;
            return this;
        }

        public void PluginUnload()
        {
#if FEATURE_UNLOAD
            this.privateAssemblies = null;
            this.defaultAssemblies = null;
            this.defaultLoadContext = null;
            this.mainAssemblyPath = null;
#endif
        }
    }
#endif
}
