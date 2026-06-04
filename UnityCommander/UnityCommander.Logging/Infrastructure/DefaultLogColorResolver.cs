
using UnityCommander.Logging.Coloring;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Abstractions
{
    public sealed class DefaultLogColorResolver : ILogColorResolver
    {
        public LogColor Resolve(LogEntry entry)
        {
            if (entry.Level >= LogLevel.Error)
                return LogColor.Error;

            return entry.Category switch
            {
                LogCategory.Plugin => LogColor.Plugin,
                LogCategory.Autocomplete => LogColor.Autocomplete,
                LogCategory.System => LogColor.System,
                LogCategory.UserAction => LogColor.UserAction,
                _ => LogColor.Default
            };
        }
    }
}
