using UnityCommander.Logging.Contracts;

namespace UnityCommander.Logging.Core
{
    public sealed class LoggerCore
    {
        private readonly ILogFilter _policy;
        private readonly ILogColorResolver _colorResolver;
        private readonly LogHub _hub;

        public LoggerCore(LogHub hub, ILogFilter policy, ILogColorResolver colorResolver)
        {
            _policy = policy;
            _colorResolver = colorResolver;
            _hub = hub;
        }

        public void Process(LogEntry entry)
        {
            if (entry.Condition != null && !entry.Condition())
                return;

            if (!_policy.Allow(entry))
                return;

            entry.Color ??= _colorResolver.Resolve(entry);

            _hub.Publish(entry);
        }
    }
}
