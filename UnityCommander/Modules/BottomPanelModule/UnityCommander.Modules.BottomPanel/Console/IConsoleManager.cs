
using System.Collections.Generic;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public interface IConsoleManager
    {
        IReadOnlyCollection<ConsoleSession> Sessions { get; }

        ConsoleSession Create();

        void Close(ConsoleSession session);
    }
}
