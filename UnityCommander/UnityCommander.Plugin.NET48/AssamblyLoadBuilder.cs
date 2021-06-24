using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UnityCommander.Plugin.NET48
{
    public class AssemblyLoadContextBuilder
    {
        private readonly List<string> additionalProbingPaths = new();
        private readonly List<string> resourceProbingPaths = new();
        private readonly List<string> resourceProbingSubpaths = new();
        private readonly HashSet<string> privateAssemblies = new(StringComparer.Ordinal);
        private readonly HashSet<string> defaultAssemblies = new(StringComparer.Ordinal);
        private string mainAssemblyPath;
        private object defaultLoadContext;
        private bool lazyLoadReferences;

        public AssemblyLoadContextBuilder PreferDefaultLoadContextAssembly(AssemblyName assemblyName)
        {
            var names = new Queue<AssemblyName>();
            return this;
        }

        public AssemblyLoadContextBuilder SetMainAssemblyPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Argument must not be null or empty.", nameof(path));
            }

            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException("Argument must be a full path.", nameof(path));
            }

            this.mainAssemblyPath = path;
            return this;
        }

        public AssemblyLoadContextBuilder SetDefaultContext(object context)
        {
            this.defaultLoadContext = context ?? throw new ArgumentException($"Bad Argument: AssemblyLoadContext in {nameof(AssemblyLoadContextBuilder)}.{nameof(this.SetDefaultContext)} is null.");
            return this;
        }

        public object Build()
        {
            return null;
        }

        public AssemblyLoadContextBuilder IsLazyLoaded(bool isLazyLoaded)
        {
            this.lazyLoadReferences = isLazyLoaded;
            return this;
        }
    }
}
