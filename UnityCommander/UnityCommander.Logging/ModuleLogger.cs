
#define Nlog

namespace UnityCommander.Logging
{
    using System;
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
    }
}
