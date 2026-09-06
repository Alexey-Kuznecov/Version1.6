
using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Logging.Profiling;
using UnityCommander.Services.Bootstrap;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public class PluginCatalog: IPluginCatalog
    {
        private IPluginManager _manager;

        private readonly LoggerCreator _loggerCreator;

        public PluginCatalog(IPluginManager manager, LoggerCreator loggerCreator)
        {
            _loggerCreator = loggerCreator;
            _manager = manager;
        }

        public PluginInfo Get(string id)
            => _manager.GetPluginInfo(id);

        public IReadOnlyCollection<PluginInfo> GetAll()
              => _manager
                .GetAllPluginInfo();

        public void LoadMetadata()
        {
            using (_loggerCreator.ProfileScope(LogScope.Startup, "Plugin"))
            {
                _manager.LoadMetadata();
            }
        }
    }
}
