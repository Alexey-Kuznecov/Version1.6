
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Logging
{
    public static class Log
    {
        private static LoggerCreator? _loggerCreator;

        public static void Initialize(
            LoggerCreator loggerCreator)
        {
            _loggerCreator = loggerCreator;
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
    }
}
