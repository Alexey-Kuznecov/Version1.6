using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Filters
{
    public sealed class LoggingPolicyFilter : ILogFilter
    {
        private readonly GlobalLoggerSettings _settings;

        public LoggingPolicyFilter(GlobalLoggerSettings loggerSettings)
        {
            _settings = loggerSettings;
        }

        public bool Allow(LogEntry log)
        {
            // 1️⃣ Debug — всё
            if (_settings.Mode == LoggingMode.Debug)
                return true;

            // 2️⃣ Уровень
            if (log.Level < _settings.MinimumLevel)
                return false;

            // 3️⃣ Scope — строгий фильтр источника
            if (_settings.EnabledScopes != null &&
                !_settings.EnabledScopes.Contains(log.Scope))
                return false;

            // 4️⃣ Режимы (по категориям)
            return _settings.Mode switch
            {
                LoggingMode.UserActions =>
                    log.Category == LogCategory.UserAction,

                LoggingMode.Information =>
                    log.Level >= LogLevel.Info
                    && log.Category != LogCategory.Performance,

                LoggingMode.ErrorsOnly =>
                    log.Level >= LogLevel.Error,

                _ => true
            };
        }
    }
}
