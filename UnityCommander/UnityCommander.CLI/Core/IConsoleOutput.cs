
using UnityCommander.CLI.Infrastructure;

namespace UnityCommander.CLI.Core
{
    public interface IConsoleOutput
    {
        event Action<IConsoleActivityState?>? ActivityChanged;

        event Action<string>? TextWritten;
        event Action? Cleared;

        void Write(string text);
        void WriteLine(string text);
        void WriteError(string message);
        void WriteWarning(string message);
        void WriteSuccess(string message);
        void Clear();

        IConsoleActivity StartActivity(string message);
    }
}
