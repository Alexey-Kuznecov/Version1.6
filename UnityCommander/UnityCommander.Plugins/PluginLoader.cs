using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Plugins
{
    internal class PluginLoader
    {
        public IPluginFactory? LoadPlugin(string path)
        {
            if (!File.Exists(path)) return null;

            var assembly = Assembly.LoadFrom(path);
            var factoryType = assembly.GetTypes().FirstOrDefault(t => typeof(IPluginFactory).IsAssignableFrom(t) && !t.IsAbstract);
            if (factoryType == null) return null;

            return (IPluginFactory?)Activator.CreateInstance(factoryType);
        }
    }
}
