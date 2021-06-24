
namespace UnityCommander.Plugin.NETCore
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Loader;

    public class PluginConfig
    {
        public PluginConfig(string assemblyFile)
        {
            this.MainAssemblyPath = assemblyFile;
        }

        public bool IsLazyLoaded { get; set; } = true;
        public string MainAssemblyPath { get; set; }
        public ICollection<AssemblyName> SharedAssemblies { get; protected set; } = new List<AssemblyName>();
        public AssemblyLoadContext DefaultContext { get; set; } = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;
    }
}
