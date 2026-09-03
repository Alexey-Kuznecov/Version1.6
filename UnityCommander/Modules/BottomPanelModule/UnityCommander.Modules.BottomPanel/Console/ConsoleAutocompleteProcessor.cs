using System;
using System.Windows;
using System.Windows.Threading;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Infrastructure.Analyze;
using UnityCommander.Autocomplete.Input;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public class ConsoleAutocompleteProcessor
    {
        private readonly ICliInputAnalyzer _cliInputAnalyzer;
        private readonly ICliParseStateBuilder _parseStateBuilder;
        private readonly ILogger _logger;
        private readonly ICompletionEngine _completionEngine;

        public ConsoleAutocompleteProcessor(
            ICompletionEngine completionEngine,
            ICliInputAnalyzer cliInputAnalyzer,
            ICliParseStateBuilder parseStateBuilder, 
            LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<ConsoleAutocompleteProcessor>(LogScope.Runtime);
            _completionEngine = completionEngine;
            _cliInputAnalyzer = cliInputAnalyzer;
            _parseStateBuilder = parseStateBuilder;
        }

        public void UpdateCompletions(ConsoleState consoleState)
        {
            if (string.IsNullOrEmpty(consoleState.InputText))
            {
                ClearCompletions(consoleState);
                return;
            }

            var caret = Math.Max(0, consoleState.CaretIndex);

            var inputStatus = _cliInputAnalyzer.Analyze(
                consoleState.InputText,
                caret);

            var parseState = _parseStateBuilder.Build(inputStatus);
            var state = new InputState(consoleState.InputText, caret);

            if (parseState.IsEditingToken)
                return;

            var result = _completionEngine.GetCompletions(
                state,
                parseState);

            consoleState.Completions.Clear();

            if (result == null)
                return;

            foreach (var item in result.Items)
                consoleState.Completions.Add(item);

            consoleState.SelectedIndex = result.DefaultSelectedIndex;
        }

        public bool CanAccept(ConsoleState consoleState) =>
             consoleState.SelectedIndex >= 0 && consoleState.SelectedIndex < consoleState.Completions.Count;

        public void Accept(ConsoleState consoleState)
        {
            if (!CanAccept(consoleState))
                return;

            consoleState.SuppressCompletionUpdate = true;

            try
            {
                if (string.IsNullOrEmpty(consoleState.InputText))
                    return;

                var inputText = consoleState.InputText;
                var caretIndex = consoleState.CaretIndex;
                var selectedIndex = consoleState.SelectedIndex;

                _logger.Debug(
                    $"[Completion] Snapshot: " +
                    $"Input='{inputText}', " +
                    $"Length={inputText?.Length}, " +
                    $"Caret={caretIndex}, " +
                    $"Selected={selectedIndex}");

                var state = new InputState(consoleState.InputText, consoleState.CaretIndex - 1);
                var item = consoleState.Completions[consoleState.SelectedIndex];

                var edit = _completionEngine.ApplyCompletion(state, item);


                if (edit.ReplaceStart < 0 ||
                    edit.ReplaceLength < 0 ||
                    edit.ReplaceStart + edit.ReplaceLength > inputText.Length)
                {
                    _logger.Error(
                        $"[Completion] INVALID EDIT! " +
                        $"Input='{inputText}', " +
                        $"InputLength={inputText.Length}, " +
                        $"Caret={caretIndex}, " +
                        $"SelectedIndex={selectedIndex}, " +
                        $"ReplaceStart={edit.ReplaceStart}, " +
                        $"ReplaceLength={edit.ReplaceLength}, " +
                        $"InsertText='{edit.InsertText}', " +
                        $"CaretOffset={item.CaretOffset}");

                    return;
                }

                consoleState.InputText = consoleState.InputText.Substring(0, edit.ReplaceStart)
                                         + edit.InsertText
                                         + consoleState.InputText.Substring(edit.ReplaceStart + edit.ReplaceLength);

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    consoleState.CaretIndex = edit.ReplaceStart + edit.InsertText.Length + item.CaretOffset;
                }, DispatcherPriority.Background);

                ClearCompletions(consoleState);
            }
            finally
            {
                consoleState.SuppressCompletionUpdate = false;
            }
        }

        public void ClearCompletions(ConsoleState consoleState)
        {
            consoleState.Completions.Clear();
            consoleState.SelectedIndex = -1;
        }
    }
}
