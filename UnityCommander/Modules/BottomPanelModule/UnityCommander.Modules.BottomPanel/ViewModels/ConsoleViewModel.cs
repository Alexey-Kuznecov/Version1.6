using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Infrastructure.Analyze;
using UnityCommander.Autocomplete.Input;
using UnityCommander.CLI.Commands;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Helper;

using UnityCommander.CLI.Integration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class ConsoleViewModel : BindableBase
    {
        private readonly ICliInputAnalyzer _cliInputAnalyzer;
        private readonly ICliParseStateBuilder _parseStateBuilder;
        private readonly ILogger _logger;
        private readonly IConsoleInput _input;
        private readonly IConsoleOutput _output;
        private readonly ConsoleCommandDispatcher _dispatcher;
        private readonly IServiceProvider _services;
        private readonly ConsoleApplicationLifetime _lifetime;
        private readonly IConsoleCommandProvider _consoleCommandProvider;
        private readonly IPluginProvider _pluginProvider;
        private readonly IConsoleAutoComplete _autoComplete;
        private readonly ICompletionEngine _completionEngine;
        private bool _suppressCompletionUpdate;
        private string _lastTokenValue;
        private InputToken _lastInputToken;

        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                if (SetProperty(ref _inputText, value))
                {
                    if (!_suppressCompletionUpdate)
                        UpdateCompletions();
                }
            }
        }

        private int _caretIndex;
        public int CaretIndex
        {
            get => _caretIndex;
            set => SetProperty(ref _caretIndex, value);
        }

        private int _selectedIndex = -1;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        private readonly ObservableCollection<CompletionItem> _completions = new();
        public ReadOnlyObservableCollection<CompletionItem> Completions { get; }

        private readonly ObservableCollection<string> _lines = new();
        public ReadOnlyObservableCollection<string> Lines { get; }

        public DelegateCommand SendCommandCommand { get; }
        public DelegateCommand CopyCommand => new DelegateCommand(() =>
        {
            var text = string.Join(Environment.NewLine, Lines);
            Clipboard.SetText(text);
        });

        public ICommand NavigateUpCommand { get; }
        public ICommand NavigateDownCommand { get; }
        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }

        public ConsoleViewModel(
            IConsoleInput input,
            IConsoleOutput output,
            ConsoleCommandDispatcher dispatcher,
            IServiceProvider services,
            ConsoleApplicationLifetime lifetime,
            IEventAggregator ea, 
            IConsoleCommandProvider consoleCommandProvider,
            IPluginProvider pluginProvider,
            ICompletionEngine completionEngine,
            LoggerCreator loggerCreator,
            ICliInputAnalyzer cliInputAnalyzer,
            ICliParseStateBuilder parseStateBuilder) //, IPluginProvider pluginProvider)
        {
            _cliInputAnalyzer = cliInputAnalyzer;
            _parseStateBuilder = parseStateBuilder;
            _logger = loggerCreator.For<ConsoleViewModel>(LogScope.UI);
            _input = input;
            _output = output;
            _dispatcher = dispatcher;
            _services = services;
            _lifetime = lifetime;
            _consoleCommandProvider = consoleCommandProvider;
            _pluginProvider = pluginProvider;
            _completionEngine = completionEngine;

            Completions = new ReadOnlyObservableCollection<CompletionItem>(_completions);

            AcceptCommand = new DelegateCommand(Accept, CanAccept)
                .ObservesProperty(() => SelectedIndex);

            CancelCommand = new DelegateCommand(ClearCompletions);
            
            // Регистрируем все команды из сервиса
            foreach (var cmd in _consoleCommandProvider.GetAllCommands())
            {
                _dispatcher.RegisterCommand(cmd);
            }

            Lines = new ReadOnlyObservableCollection<string>(_lines);
            // САМОЕ ВАЖНОЕ: подписка на UI-потоке
            ea.GetEvent<ConsoleWriteEvent>().Subscribe(text =>
            {
                Application.Current.Dispatcher.Invoke(() => AppendLine(text));
            });

            SendCommandCommand = new DelegateCommand(SendInput);

            InputText = string.Empty;
            CaretIndex = 1;
            Task.Run(MainLoop);
            _pluginProvider = pluginProvider;
        }

        private void AppendLine(string text)
        {
            _lines.Add(text);
        }

        private void SendInput()
        {
            var text = InputText;
            InputText = "";
            _input.Submit(text);
        }

        private async Task MainLoop()
        {
            _output.WriteLine("Unity Commander Internal Console ready.");

            while (_lifetime.IsRunning)
            {
                var line = await _input.ReadLineAsync(CancellationToken.None);
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = ParseHelper.ParseArguments(line);
                var name = parts[0];
                var args = parts.Skip(1).ToArray();

                var ctx = new ConsoleCommandContext(_services, _output, args);

                await _dispatcher.ExecuteCommandAsync(name, ctx, CancellationToken.None);
            }
        }

        private void UpdateCompletions()
        {
            var caret = Math.Max(0, CaretIndex);
            var inputStatus = _cliInputAnalyzer.Analyze(InputText, caret);
            var parseState = _parseStateBuilder.Build(inputStatus);
            var state = new InputState(InputText, caret);
            //if (parseState.IsEditingToken)
            //    return;
            var result = _completionEngine.GetCompletions(state, parseState);
            //var token = _completionEngine.GetTokenNearCaret(InputText, CaretIndex);

            _completions.Clear();

            _logger.Info($"UpdateCompletions [CaretPosition={state.CaretPosition}] [InputText={InputText}]");
            if (result == null)
                return;

            //_logger.ObjectInfo("UpdateCompletions ", token);
            foreach (var item in result.Items)
                _completions.Add(item);
            SelectedIndex = result.DefaultSelectedIndex;
            //CaretIndex = CaretIndex < InputText.Length ? InputText.Length : CaretIndex;
            //RaisePropertyChanged(nameof(CaretIndex));
        }

        private bool CanAccept() =>
            SelectedIndex >= 0 && SelectedIndex < _completions.Count;

        private void Accept()
        {
            if (!CanAccept())
                return;
            _suppressCompletionUpdate = true;

            try
            {
                if (string.IsNullOrEmpty(InputText))
                    return;

                var state = new InputState(InputText, CaretIndex -1);
                var item = _completions[SelectedIndex];

                var edit = _completionEngine.ApplyCompletion(state, item);
                _logger.Info($"AcceptCompletions [CaretPosition={state.CaretPosition}] [InputText={InputText}]");
                _logger.ObjectInfo("AcceptCompletions ", edit.CurrentToken);
                // Заменяем только нужный диапазон
                InputText = InputText.Substring(0, edit.ReplaceStart)
                            + edit.InsertText
                            + InputText.Substring(edit.ReplaceStart + edit.ReplaceLength);

                // Ставим каретку после вставленного текста
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CaretIndex = edit.ReplaceStart + edit.InsertText.Length;
                }, DispatcherPriority.Background);

                ClearCompletions();
            }
            finally
            {
                //_lastInputToken = _completionEngine.GetTokenNearCaret(InputText, CaretIndex);
                _suppressCompletionUpdate = false;
                _lastTokenValue = InputText;
            }
        }

        private void ClearCompletions()
        {
            _completions.Clear();
            SelectedIndex = -1;
        }
    }
}
