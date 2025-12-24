
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
using UnityCommander.CLI.Autocomplete;
using UnityCommander.CLI.Commands;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Helper;
using UnityCommander.CLI.Integration;
using UnityCommander.Core.Navgator;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class ConsoleViewModel : BindableBase
    {
        private readonly IConsoleInput _input;
        private readonly IConsoleOutput _output;
        private readonly ConsoleCommandDispatcher _dispatcher;
        private readonly IServiceProvider _services;
        private readonly ConsoleApplicationLifetime _lifetime;
        private readonly IConsoleCommandProvider _consoleCommandProvider;
        private readonly IPluginProvider _pluginProvider;
        private readonly IConsoleAutoComplete _autoComplete;

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                if (SetProperty(ref _inputText, value))
                {
                    UpdateSuggestions();
                    MoveCaretToEnd();
                }
            }
        }

        private int _selectedSuggestionIndex = -1;
        public int SelectedSuggestionIndex
        {
            get => _selectedSuggestionIndex;
            set => SetProperty(ref _selectedSuggestionIndex, value);
        }

        private int _caretIndex;
        public int CaretIndex
        {
            get => _caretIndex;
            set => SetProperty(ref _caretIndex, value);
        }

        private readonly ObservableCollection<string> _suggestions = new();
        public ReadOnlyObservableCollection<string> Suggestions { get; }

        private readonly ObservableCollection<string> _lines = new();
        public ReadOnlyObservableCollection<string> Lines { get; }

        public DelegateCommand SendCommandCommand { get; }
        public DelegateCommand CopyCommand => new DelegateCommand(() =>
        {
            var text = string.Join(Environment.NewLine, Lines);
            Clipboard.SetText(text);
        });

        private bool CanNavigate() => Suggestions.Count > 0;

        public ICommand NavigateUpCommand { get; }
        public ICommand NavigateDownCommand { get; }
        public ICommand AcceptSuggestionCommand { get; }
        public ICommand CancelSuggestionCommand { get; }

        public ConsoleViewModel(
            IConsoleInput input,
            IConsoleOutput output,
            ConsoleCommandDispatcher dispatcher,
            IServiceProvider services,
            ConsoleApplicationLifetime lifetime,
            IEventAggregator ea, 
            IConsoleCommandProvider consoleCommandProvider,
            IPluginProvider pluginProvider, 
            IConsoleAutoComplete autoComplete) //, IPluginProvider pluginProvider)
        {
            _input = input;
            _output = output;
            _dispatcher = dispatcher;
            _services = services;
            _lifetime = lifetime;
            _consoleCommandProvider = consoleCommandProvider;
            _pluginProvider = pluginProvider;
            _autoComplete = autoComplete;
            Suggestions = new ReadOnlyObservableCollection<string>(_suggestions);
            NavigateUpCommand = new DelegateCommand(OnNavigateUp, CanNavigate);
            NavigateDownCommand = new DelegateCommand(OnNavigateDown, CanNavigate);
            AcceptSuggestionCommand = new DelegateCommand(OnAccept, CanNavigate);
            CancelSuggestionCommand = new DelegateCommand(OnCancel);
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

        private void UpdateSuggestions()
        {
            _suggestions.Clear();

            var parts = InputText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                _suggestions.Clear();
                return;
            }

            var cmdName = parts[0];
            var args = parts.Skip(1).ToArray();

            // Проверяем, это автокомплит команд или аргументов
            if (args.Length == 0)
            {
                // Автокомплит команд
                foreach (var c in _dispatcher.GetAvailableCommands()
                                             .Select(c => c.Name)
                                             .Where(name => name.StartsWith(cmdName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (cmdName == c)
                    {
                        _suggestions.Clear();
                        return;
                    }

                    _suggestions.Add(c);
                }
            }
            else
            {
                if (_dispatcher.TryGetCommand(cmdName, out var cmd) && cmd is IAutoCompleteArgumentsProvider argProvider)
                {
                    var lastArg = args.Length > 0 ? args.Last() : "";
                    var suggestions = argProvider.GetArgumentSuggestions(args).ToList();

                    // 🔥 если последний аргумент полностью совпадает с одним из вариантов → список скрываем
                    if (suggestions.Any() && suggestions.Contains(lastArg, StringComparer.OrdinalIgnoreCase))
                    {
                        _suggestions.Clear();
                        return;
                    }

                    // иначе показываем список частичных совпадений
                    foreach (var s in argProvider.GetArgumentSuggestions(args))
                    {
                        if (s.StartsWith(args.Last(), StringComparison.OrdinalIgnoreCase))
                            _suggestions.Add(s);
                    }
                }
            }

            // Выбираем элемент по умолчанию
            if (Suggestions.Count > 0)
            {
                // Выделяем последний элемент через Dispatcher, чтобы ListBox успел обновиться
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    SelectedSuggestionIndex = Suggestions.Count - 1;
                }, DispatcherPriority.Background);
            }
            else
            {
                SelectedSuggestionIndex = -1;
            }
        }
        
        private void OnNavigateUp()
        {
            SelectedSuggestionIndex =
                Math.Max(SelectedSuggestionIndex - 1, 0);
        }

        private void OnNavigateDown()
        {
            SelectedSuggestionIndex =
                Math.Min(SelectedSuggestionIndex + 1, Suggestions.Count - 1);
        }

        private void OnAccept()
        {
            if (SelectedSuggestionIndex < 0 || SelectedSuggestionIndex >= Suggestions.Count)
                return;

            var selected = Suggestions[SelectedSuggestionIndex];

            var parts = InputText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
            {
                // вставка команды
                InputText = selected + " ";
            }
            else
            {
                // вставка аргумента
                parts[parts.Length - 1] = selected;
                InputText = string.Join(" ", parts) + " ";
            }

            // 🔥 каретка теперь реально будет в конце
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                CaretIndex = InputText.Length;
            }, DispatcherPriority.Render);

            _suggestions.Clear();
            SelectedSuggestionIndex = -1;
        }

        private void OnCancel()
        {
            _suggestions.Clear();
            SelectedSuggestionIndex = -1;
        }

        private void MoveCaretToEnd()
        {
            _caretIndex = InputText.Length;
            if (InputText.Length == CaretIndex)
                RaisePropertyChanged(nameof(CaretIndex)); // форсируем уведомление
        }
    }
}
