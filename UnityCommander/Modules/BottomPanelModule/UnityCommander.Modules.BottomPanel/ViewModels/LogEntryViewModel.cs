
using System.Collections.Generic;
using System.Windows.Documents;
using UnityCommander.Logging.Abstractions;

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
