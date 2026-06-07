using UnityCommander.Logging.Contracts;

namespace UnityCommander.Logging.Core
{
    public sealed class LoggerFactory
    {
        private readonly LoggerCore _core;

        public LoggerFactory(LoggerCore core)
        {
            _core = core;
        }

        public ILogger Create(string category, string scope)
            => new Logger(_core, category, scope);

        public ILogger CreateFor<T>()
            => Create(typeof(T).Name, "System");

        public ILogger CreateForPlugin(string pluginId)
            => Create("Plugin", pluginId);
    }
}
