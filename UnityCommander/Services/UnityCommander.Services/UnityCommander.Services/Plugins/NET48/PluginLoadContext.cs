
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UnityCommander.Services.Plugins.NET48
{
#if NET472
    public class PluginLoadContext
    {
        private readonly HashSet<string> defaultAssemblies;
        private readonly object defaultLoadContext;
        private readonly HashSet<string> privateAssemblies;

        public PluginLoadContext(
            string pluginPath, 
            object defaultLoadContext,
            HashSet<string> defaultAssemblies)
        {
            this.defaultLoadContext = defaultLoadContext;
            this.defaultAssemblies = defaultAssemblies;
        }

        protected Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }

        protected IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return IntPtr.Zero;
        }

        public Assembly LoadAssemblyFromFilePath(string path)
        {
            return null;
        }
    }
#endif
}
