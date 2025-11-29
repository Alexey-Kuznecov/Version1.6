
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
#endif
        public ModuleLogger()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.Configuration = new XmlLoggingConfiguration($"{dir}\\NLog.config");
        }

        public Logger GetLogger() => Logger;

        public void Log(
                        string message, 
                        [CallerLineNumber] int lineNumber = 0,
                        [CallerMemberName] string caller = null,
                        [CallerFilePath] string file = null)
        {
            Logger.Info(message + " at line " + lineNumber + " (" + caller + ")");
        }

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
