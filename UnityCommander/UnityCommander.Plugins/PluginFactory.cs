using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Plugins
{
    internal class PluginFactory : IPluginFactory
    {
        private readonly Type _pluginType;

        public PluginFactory(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public IPlugin CreatePlugin()
        {
            return (IPlugin)Activator.CreateInstance(_pluginType)!;
        }
    }
}
