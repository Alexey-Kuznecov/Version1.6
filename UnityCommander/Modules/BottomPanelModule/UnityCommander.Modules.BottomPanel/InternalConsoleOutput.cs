
using System;
using UnityCommander.CLI.Core;

namespace UnityCommander.Modules.BottomPanel
{
    public sealed class InternalConsoleOutput : IConsoleOutput
    {
        public event Action<string>? TextWritten;
        public event Action? Cleared;

        public void Write(string text)
            => TextWritten?.Invoke(text);

        public void WriteLine(string text)
            => TextWritten?.Invoke(text + Environment.NewLine);

        public void WriteError(string message)
            => WriteLine("[ERROR] " + message);

        public void WriteWarning(string message)
            => WriteLine("[WARNING] " + message);

        public void WriteSuccess(string message)
            => WriteLine("[OK] " + message);

        public void Clear()
            => Cleared?.Invoke();
    }
}
