using System.Collections.Generic;
using UnityCommander.Logging.Core;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public sealed class LogEntryViewModel
    {
        public IReadOnlyList<LogInline> Parts { get; }

        public LogEntryViewModel(LogEntry entry, ILogHighlighter highlighter)
        {
            Parts = highlighter.Build(entry);
        }
    }
}
