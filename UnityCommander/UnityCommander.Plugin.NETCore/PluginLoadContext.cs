
#nullable enable
using System.Collections.Generic;

namespace UnityCommander.Plugin.NETCore
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Loader;

    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyLoadContext defaultLoadContext;
        private string mainAssemblyPath;
        private readonly HashSet<string> defaultAssemblies;
        private readonly HashSet<string> privateAssemblies;
        private readonly bool lazyLoadReferences;

        public PluginLoadContext(
            string mainAssemblyPath, 
            AssemblyLoadContext defaultLoadContext,
            HashSet<string> defaultAssemblies,
            HashSet<string> privateAssemblies,
            bool lazyLoadReferences,
            bool isCollectible)

            : base(Path.GetFileNameWithoutExtension(mainAssemblyPath), isCollectible)
        {
            this.defaultLoadContext = defaultLoadContext;
            this.privateAssemblies = privateAssemblies ?? throw new ArgumentNullException(nameof(privateAssemblies));
            this.defaultAssemblies = defaultAssemblies;
            this.lazyLoadReferences = lazyLoadReferences;
            this.mainAssemblyPath = mainAssemblyPath ?? throw new ArgumentNullException(nameof(mainAssemblyPath));
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == null)
            {
                return null;
            } 

            if (this.defaultAssemblies.Contains(assemblyName.Name) && !this.privateAssemblies.Contains(assemblyName.Name))
            {
                try
                {
                    var defaultAssembly = this.defaultLoadContext.LoadFromAssemblyName(assemblyName);
                    if (this.lazyLoadReferences)
                    {
                        foreach (var reference in defaultAssembly.GetReferencedAssemblies())
                        {
                            if (reference.Name != null && !this.defaultAssemblies.Contains(reference.Name))
                            {
                               this.defaultAssemblies.Add(reference.Name);
                            }
                        }
                    }
                    return defaultAssembly;
                }
                catch
                {
                    // Swallow errors in loading from the default context
                }
            }

            return null;
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
    }
}
