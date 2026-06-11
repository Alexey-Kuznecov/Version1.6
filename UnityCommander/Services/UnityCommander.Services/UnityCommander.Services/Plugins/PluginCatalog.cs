
using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Profiling;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public class PluginCatalog: IPluginCatalog
    {
        private LogHub _hub;

        private IPluginManager _manager;

        public PluginCatalog(IPluginManager manager, LogHub hub)
        {
            _hub = hub;
            _manager = manager;
        }

        public PluginInfo Get(string id)
            => _manager.GetPluginInfo(id);

        public IReadOnlyCollection<PluginInfo> GetAll()
              => _manager
                .GetAllPluginInfo();

        public void LoadMetadata()
        {
            using (new LogScopeTimer(_hub, LogScope.Startup, "Plugin"))
            {
                _manager.LoadMetadata();
            }
        }
    }
}
