using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Plugins
{
    internal class PluginContainer
    {
        private readonly List<IPlugin> _plugins = new();

        public IReadOnlyList<IPlugin> Plugins => _plugins;

        public void AddPlugin(IPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        public void RemovePlugin(IPlugin plugin)
        {
            _plugins.Remove(plugin);
        }

        public void Clear()
        {
            _plugins.Clear();
        }
    }
}
