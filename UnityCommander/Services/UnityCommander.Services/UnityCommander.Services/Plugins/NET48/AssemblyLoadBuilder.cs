using System;
using System.Collections.Generic;
using System.Reflection;


namespace UnityCommander.Services.Plugins.NET48
{
#if NET472
    internal class AssemblyLoadBuilder
    {
        private readonly HashSet<string> privateAssemblies = new(StringComparer.Ordinal);
        private readonly HashSet<string> defaultAssemblies = new(StringComparer.Ordinal);
        private string mainAssemblyPath;
        public object defaultLoadContext;

        public object Build()
        {
            return new PluginLoadContext(
                this.mainAssemblyPath,
                this.defaultLoadContext,
                this.defaultAssemblies);
        }

        public AssemblyLoadBuilder PreferDefaultLoadContextAssembly(AssemblyName assemblyName)
        {
            if (assemblyName.Name != null && !this.defaultAssemblies.Contains(assemblyName.Name))
            {
                this.defaultAssemblies.Add(assemblyName.Name);
               // var assembly = this.defaultLoadContext.LoadFromAssemblyName(assemblyName);
                //foreach (var reference in assembly.GetReferencedAssemblies())
                //{
                //    if (reference.Name != null)
                //    {
                //        this.defaultAssemblies.Add(reference.Name);
                //    }
                //}
            }

            return this;
        }

        public AssemblyLoadBuilder SetDefaultContext(object context)
        {
            this.defaultLoadContext = context ?? throw new ArgumentException($"Bad Argument: AssemblyLoadContext in {nameof(AssemblyLoadBuilder)}.{nameof(this.SetDefaultContext)} is null.");
            return this;
        }

        public AssemblyLoadBuilder SetMainAssemblyPath(string path)
        {
            this.mainAssemblyPath = path;
            return this;
        }
    }
#endif
}
