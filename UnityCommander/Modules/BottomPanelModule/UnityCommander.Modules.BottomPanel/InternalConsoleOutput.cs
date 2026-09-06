
using System;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Infrastructure;
using UnityCommander.Modules.BottomPanel.Console;

namespace UnityCommander.Modules.BottomPanel
{
    public sealed class InternalConsoleOutput : IConsoleOutput
    {
        private IConsoleActivityState _activity;

        public event Action<IConsoleActivityState?>? ActivityChanged;

        public event Action<string>? TextWritten;
        public event Action? Cleared;

        public InternalConsoleOutput()
        {
        }

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

        public IConsoleActivity StartActivity(string message)
        {
            _activity = new ConsoleActivityState
            {
                Title = message,
                Status = message,
            };

            ActivityChanged?.Invoke(_activity);

            return new ConsoleActivity(this);
        }

        internal void UpdateActivity(Action<IConsoleActivityState> update)
        {
            if (_activity == null)
                return;

            update.Invoke(_activity);

            ActivityChanged?.Invoke(_activity);
        }

        internal void CompleteActivity()
        {
            _activity = null;

            ActivityChanged?.Invoke(null);
        }
    }
}
