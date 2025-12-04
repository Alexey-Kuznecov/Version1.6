using NLog.Config;
using NLog.Targets;
using NLog;

namespace UnityCommander.Logging
{
    /// <summary>
    /// Класс для конфигурации логгера NLog.
    /// </summary>
    public static class NLogSettings
    {
        private static bool _isConfigured;

        /// <summary>
        /// Настраивает логгер с указанным путём к лог-файлу.
        /// </summary>
        /// <param name="logDirectory">Папка для логов. Например, "Logs".</param>
        /// <param name="fileName">Имя файла без расширения. По умолчанию: "app".</param>
        public static void Configure(string logDirectory = "Logs", string fileName = "app")
        {
            if (_isConfigured) return;

            var fullLogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logDirectory);
            Directory.CreateDirectory(fullLogDirectory); // гарантируем, что папка есть

            var logFilePath = Path.Combine(fullLogDirectory, $"{fileName}_${{shortdate}}.log");

            var config = new LoggingConfiguration();

            // Основной файл логов
            var fileTarget = new FileTarget("logfile")
            {
                FileName = logFilePath,
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${exception:format=ToString}",
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 10,
                Encoding = System.Text.Encoding.UTF8,
                KeepFileOpen = false
            };

#if DEBUG
            // Консоль для режима отладки
            var consoleTarget = new ColoredConsoleTarget("console")
            {
                Layout = "${longdate}|${level}|${message}"
            };
            config.AddTarget(consoleTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);
#endif

            config.AddTarget(fileTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);

            LogManager.Configuration = config;
            _isConfigured = true;
        }
    }
}
