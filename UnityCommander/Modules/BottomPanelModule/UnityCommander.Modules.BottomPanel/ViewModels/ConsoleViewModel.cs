
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UnityCommander.CLI.Commands;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Helper;
using UnityCommander.CLI.Integration;
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

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        private readonly ObservableCollection<string> _lines = new();
        public ReadOnlyObservableCollection<string> Lines { get; }

        public DelegateCommand SendCommandCommand { get; }
        public DelegateCommand CopyCommand => new DelegateCommand(() =>
        {
            var text = string.Join(Environment.NewLine, Lines);
            Clipboard.SetText(text);
        });

        public ConsoleViewModel(
            IConsoleInput input,
            IConsoleOutput output,
            ConsoleCommandDispatcher dispatcher,
            IServiceProvider services,
            ConsoleApplicationLifetime lifetime,
            IEventAggregator ea, 
            IConsoleCommandProvider consoleCommandProvider)
        {
            _input = input;
            _output = output;
            _dispatcher = dispatcher;
            _services = services;
            _lifetime = lifetime;
            _consoleCommandProvider = consoleCommandProvider;

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
    }
}
