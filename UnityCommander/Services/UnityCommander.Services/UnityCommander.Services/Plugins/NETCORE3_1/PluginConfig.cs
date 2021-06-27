
using System.Collections.Generic;
using System.Reflection;

namespace UnityCommander.Services.Plugins.NETCORE3_1
{
#if NETCOREAPP3_1
    using System.Runtime.Loader;
    public class PluginConfig
    {
        public PluginConfig(string assemblyFile)
        {
            this.MainAssemblyPath = assemblyFile;
        }
        public string MainAssemblyPath { get; set; }
        public HashSet<AssemblyName> SharedAssemblies { get; protected set; } = new HashSet<AssemblyName>();

        public AssemblyLoadContext DefaultContext { get; set; } = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;

        public void PluginUnload()
        {
#if FEATURE_UNLOAD
            this.SharedAssemblies = null;
            this.DefaultContext = null;
#endif
        }
    }
#endif
}
