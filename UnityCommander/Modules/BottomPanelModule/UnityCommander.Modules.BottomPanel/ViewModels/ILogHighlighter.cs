
using System.Collections.Generic;
using System.Windows.Documents;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public interface ILogHighlighter
    {
        IReadOnlyList<LogInline> Build(LogEntry entry);
    }
}
