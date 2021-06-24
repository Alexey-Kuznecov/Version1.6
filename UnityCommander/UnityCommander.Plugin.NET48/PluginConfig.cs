using System.Collections.Generic;
using System.Reflection;

namespace UnityCommander.Plugin.NET48
{

    public class PluginConfig
    {
        public PluginConfig(string assemblyFile)
        {
            this.MainAssemblyPath = assemblyFile;
        }

        public string MainAssemblyPath { get; set; }
        public ICollection<AssemblyName> SharedAssemblies { get; protected set; } = new List<AssemblyName>();
        public object DefaultContext { get; set; }
        public bool IsLazyLoaded { get; set; } = true;
    }
}
