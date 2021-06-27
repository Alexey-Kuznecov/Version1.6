using System.Collections.Generic;
using System.Reflection;

namespace UnityCommander.Services.Plugins.NET48
{
#if NET472
    public class PluginConfig
    {
        public PluginConfig(string assemblyFile)
        {
            this.MainAssemblyPath = assemblyFile;
        }
        public string MainAssemblyPath { get; set; }
        public HashSet<AssemblyName> SharedAssemblies { get; protected set; } = new HashSet<AssemblyName>();
        public object DefaultContext { get; set; }
    }
#endif
}
