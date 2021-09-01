
#define Nlog

namespace UnityCommander.Core
{
    using System;

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
    }
}
