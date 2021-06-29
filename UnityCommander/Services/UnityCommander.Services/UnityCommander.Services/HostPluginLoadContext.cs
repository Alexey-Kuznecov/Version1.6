using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace UnityCommander.Services
{
    public class HostPluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver resolver;

        public HostPluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            this.resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == null)
            {
                return null;
            }

            string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                Debug.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext", "Plugin");
                return LoadFromAssemblyPath(assemblyPath);
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
