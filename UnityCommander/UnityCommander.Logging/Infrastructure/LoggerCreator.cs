using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Profiling;

namespace UnityCommander.Logging.Infrastructure
{
    public sealed class LoggerCreator
    {
        private readonly LoggerCore _core;
        private readonly LogHub _hub;

        public LoggerCreator(
            LogHub hub,
            ILogFilter policy,
            ILogColorResolver colorResolver)
        {
            _hub = hub;
            _core = new LoggerCore(hub, policy, colorResolver);
        }

        public ILogger Create(string category, LogScope scope)
        {
            if (scope.Equals(default))
                scope = LogScope.UI;

            return new Logger(_core, category, scope.ToString());
        }

        public ILogger For<T>(LogScope scope = default)
        {
            return Create(typeof(T).Name, scope);
        }

        public ILogger ForPlugin()
            => Create("Plugin", LogScope.Plugin());

        public IDisposable ProfileScope(LogScope scope, string name)
            => new LogScopeTimer(_core, scope, name);
    }
}
