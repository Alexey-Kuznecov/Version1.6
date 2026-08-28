
using Prism.Ioc;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces;

namespace UnityCommander
{
    internal static class LoggingBootstrap
    {
        public static ILogger Initialize(IContainerProvider provider)
        {
            // Важно: принудительно создаём sink-инфраструктуру
            // до начала инициализации остальных модулей.
            provider.Resolve<ILoggingSinkService>();

            var loggerCreator = provider.Resolve<LoggerCreator>();

            Log.Initialize(loggerCreator);

            return Log.Create(
                "EarlyLoadModule",
                LogScope.Startup);
        }
    }
}
