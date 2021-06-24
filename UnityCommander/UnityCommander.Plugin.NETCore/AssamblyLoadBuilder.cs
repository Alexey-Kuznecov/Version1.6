using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace UnityCommander.Plugin.NETCore
{
    public class AssemblyLoadContextBuilder
    {
        private readonly HashSet<string> privateAssemblies = new(StringComparer.Ordinal);
        private readonly HashSet<string> defaultAssemblies = new(StringComparer.Ordinal);
        private string mainAssemblyPath;
        private bool lazyLoadReferences;

        /// <summary>
        /// 
        /// </summary>
        private AssemblyLoadContext defaultLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AssemblyLoadContext Build()
        {
            return new PluginLoadContext(
                this.mainAssemblyPath,
                defaultLoadContext,
                this.defaultAssemblies,
                this.privateAssemblies,
                this.lazyLoadReferences,
                true);
        }

        /// <summary>s
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public AssemblyLoadContextBuilder PreferDefaultLoadContextAssembly(AssemblyName assemblyName)
        {
            if (this.lazyLoadReferences)
            {
                if (assemblyName.Name != null && !this.defaultAssemblies.Contains(assemblyName.Name))
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
                }

                return this;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLazyLoaded"></param>
        /// <returns></returns>
        public AssemblyLoadContextBuilder IsLazyLoaded(bool isLazyLoaded)
        {
            this.lazyLoadReferences = isLazyLoaded;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public AssemblyLoadContextBuilder SetDefaultContext(AssemblyLoadContext context)
        {
            this.defaultLoadContext = context ?? throw new ArgumentException($"Bad Argument: AssemblyLoadContext in {nameof(AssemblyLoadContextBuilder)}.{nameof(this.SetDefaultContext)} is null.");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssemblyLoadContextBuilder SetMainAssemblyPath(string path)
        {
            this.mainAssemblyPath = path;
            return this;
        }
    }
}
