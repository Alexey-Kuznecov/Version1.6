
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Logging
{
    public static class Log
    {
        private static ILogger? _logger;

        private static LoggerCreator? _loggerCreator;

        public static void Initialize(
            LoggerCreator loggerCreator)
        {
            _loggerCreator = loggerCreator;
            _logger = _loggerCreator.Create("Log", LogScope.Runtime);
        }

        public static void Debug(string massage)
        {
            _logger?.Info(massage);
        }

        public static ILogger Create(
            string category,
            LogScope scope)
        {
            if (_loggerCreator == null)
                throw new InvalidOperationException(
                    "Logger system is not initialized.");

            return _loggerCreator.Create(
                category,
                scope);
        }

        public static LoggerCreator GetLoggerCreator()
            => _loggerCreator ?? throw new InvalidOperationException(
                "LoggerCreator has not been initialized.");
    }
}
