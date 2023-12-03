
#define Nlog

namespace UnityCommander.Core
{
    using System;
    using System.Configuration;
    using System.Data.SQLite;
    using System.IO;
    using System.Runtime.CompilerServices;

    using NLog;
    using NLog.Config;

    /// <summary>
    /// The module logger.
    /// </summary>
    public class ModuleLogger
    {
#if (Nlog)
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleLogger"/> class.
        /// </summary>
        public ModuleLogger()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.Configuration = new XmlLoggingConfiguration($"{dir}\\NLog.config");
        }

        /// <summary>
        /// The get logger.
        /// </summary>
        /// <returns>
        /// The <see cref="Logger"/>.
        /// </returns>
        public Logger GetLogger() => Logger;

        /// <summary>
        /// The show message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        public void Log(
                        string message, 
                        [CallerLineNumber] int lineNumber = 0,
                        [CallerMemberName] string caller = null,
                        [CallerFilePath] string file = null)
        {
            Logger.Info(message + " at line " + lineNumber + " (" + caller + ")");
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="logLevel">
        /// The log level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        public void Log(
                        LogLevel logLevel,
                        string message,
                        [CallerLineNumber] int lineNumber = 0,
                        [CallerMemberName] string caller = null,
                        [CallerFilePath] string file = null)
        {
            //EnsureDb();
            if (file != null)
            {
                Logger
                    .WithProperty("ModuleName", new FileInfo(file).Name + ":" + lineNumber)
                    //.WithProperty("LineNumber", lineNumber)
                    .WithProperty("CommandName", caller)
                    .Log(logLevel, message);
            }
        }

        private static void EnsureDb()
        {
            if (File.Exists("C:/Users/Lexa/Desktop/logs/logs.db"))
                return;

            using var connection = new SQLiteConnection("Data Source=C:/Users/Lexa/Desktop/logs/logs.db");
            using SQLiteCommand command = new SQLiteCommand(
                "CREATE TABLE Logging (Date TEXT, Level TEXT,ModuleName TEXT,CommandName TEXT,Message TEXT,Stacktrace TEXT)",
                connection);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
