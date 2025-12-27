
using System.Collections.Generic;
using UnityCommander.Logging.Core;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public interface ILogHighlighter
    {
        IReadOnlyList<LogInline> Build(LogEntry entry);
    }
}
