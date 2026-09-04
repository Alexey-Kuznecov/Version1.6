
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Infrastructure;

namespace UnityCommander.Copying
{
    internal class NullConsoleOutput : IConsoleOutput
    {
        public int CursorTop { get; set; }

        public int CursorLeft { get; set; }

        public int WindowWidth { get; set; }

        public event ConsoleCancelEventHandler CancelKeyPress;
        public event Action<IConsoleActivityState?>? ActivityChanged;
        public event Action<string>? TextWritten;
        public event Action? Cleared;

        public void Clear() { }
        public void SetCursorPosition(int left, int top) { }
        public void SetCursorVisible(bool isVisible) { }

        public IConsoleActivity StartActivity(string message)
        {
            throw new NotImplementedException();
        }

        public void Write(string message) { }
        public void WriteError(string message) { }
        public void WriteLine(string message) { }
        public void WriteSuccess(string message) { }
        public void WriteWarning(string message) { }
    }
}