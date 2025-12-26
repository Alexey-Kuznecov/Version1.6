
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Logging.Abstractions;
using UnityCommander.Logging.Abstractions.Helper;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class AppLogger : IAppLogger
    {
        private readonly LoggerCore _core;

        public AppLogger(
            LogHub hub,
            ILogFilter policy,
            ILogColorResolver colorResolver)
        {
            _core = new LoggerCore(hub, policy, colorResolver);
        }

        public ILogger Create(string category, LogScope scope)
        {
            if (scope.Equals(default)) scope = LogScope.UI;
            return new Logger(_core, category, scope.ToString());
        }

        public ILogger For<T>(LogScope scope = default)
        {
            return Create(typeof(T).Name, scope);
        }

        public ILogger ForPlugin(string pluginId)
            => Create("Plugin", LogScope.Plugin(pluginId));
    }
}
