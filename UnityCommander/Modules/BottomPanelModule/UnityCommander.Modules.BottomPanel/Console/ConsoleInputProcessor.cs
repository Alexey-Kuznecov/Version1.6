
using UnityCommander.CLI.History;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleInputProcessor
    {
        private readonly IConsoleHistory _history;
        
        private string? _historyDraft;

        public ConsoleInputProcessor(IConsoleHistory history)
        {
            _history = history;
        }

        public void SendInput(ConsoleSession session)
        {
            var state = session.State;

            var text = state.InputText;

            if (string.IsNullOrWhiteSpace(text))
                return;

            session.History.Add(text);

            state.InputText = string.Empty;
            state.CaretIndex = 0;

            session.Input.Submit(text);
        }

        public void NavigateDown(ConsoleState consoleState)
        {
            if (consoleState.Completions.Count > 0)
            {
                consoleState.SelectedIndex =
                    consoleState.SelectedIndex < consoleState.Completions.Count - 1
                        ? consoleState.SelectedIndex + 1
                        : 0;

                return;
            }

            NavigateHistoryDown(consoleState);
        }

        public void NavigateUp(ConsoleState consoleState)
        {
            if (consoleState.Completions.Count > 0)
            {
                consoleState.SelectedIndex =
                    consoleState.SelectedIndex > 0
                        ? consoleState.SelectedIndex - 1
                        : consoleState.Completions.Count - 1;

                return;
            }

            NavigateHistoryUp(consoleState);
        }

        private void NavigateHistoryDown(ConsoleState consoleState)
        {
            var command = _history.Next();

            if (command != null)
            {
                SetInputFromHistory(consoleState, command);
                return;
            }

            SetInputFromHistory(consoleState, _historyDraft ?? string.Empty);
            _historyDraft = null;
        }

        private void NavigateHistoryUp(ConsoleState consoleState)
        {
            if (_historyDraft == null)
                _historyDraft = consoleState.InputText;

            var command = _history.Previous();

            if (command == null)
                return;

            SetInputFromHistory(consoleState, command);
        }

        private void SetInputFromHistory(ConsoleState consoleState, string text)
        {
            consoleState.SuppressCompletionUpdate = true;

            try
            {
                consoleState.InputText = text;
                consoleState.CaretIndex = text.Length;
            }
            finally
            {
                consoleState.SuppressCompletionUpdate = false;
            }
        }
    }
}
