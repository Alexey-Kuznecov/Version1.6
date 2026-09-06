using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Logging.Filters
{
    public sealed class LoggingPolicyFilter : ILogFilter
    {
        private readonly GlobalLoggerSettings _settings;
        private readonly ILoggingRuntimeControl _runtime;

        public LoggingPolicyFilter(
            GlobalLoggerSettings loggerSettings,
            ILoggingRuntimeControl runtime)
        {
            _settings = loggerSettings;
            _runtime = runtime;
        }

        public bool Allow(LogEntry log)
        {
            if (!_runtime.IsLevelEnabled(log.Level))
                return false;

            if (!_runtime.IsCategoryEnabled(log.Category))
                return false;

            if (!_runtime.IsScopeEnabled(log.Scope))
                return false;

            if (_settings.Mode == LoggingMode.Debug)
                return true;

            if (log.Level < _settings.MinimumLevel)
                return false;

            if (_settings.EnabledScopes != null &&
                !_settings.EnabledScopes.Contains(log.Scope))
                return false;

            return _settings.Mode switch
            {
                LoggingMode.UserActions =>
                    log.Category == LogCategory.UserAction,

                LoggingMode.Information =>
                    log.Level >= LogLevel.Info &&
                    log.Category != LogCategory.Performance,

                LoggingMode.ErrorsOnly =>
                    log.Level >= LogLevel.Error,

                _ => true
            };
        }
    }
}
