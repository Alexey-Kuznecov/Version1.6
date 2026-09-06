
using System.Windows.Media;
using UnityCommander.Logging.Configuration;
using UnityCommander.Modules.BottomPanel.Highlighting;

namespace UnityCommander.Logging.Core
{
    public sealed class LogStyleResolver
    {
        public HighlightStyle Resolve(LogEntry entry)
        {
            return entry.Level switch
            {
                LogLevel.Warning => new HighlightStyle(Brushes.Yellow),
                LogLevel.Error => new HighlightStyle(Brushes.Red),
                LogLevel.Debug => new HighlightStyle(Brushes.Gray),
                LogLevel.Info => new HighlightStyle(Brushes.LightGray),
                _ => new HighlightStyle(Brushes.White)
            };
        }
    }
}
