
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.CLI.Integration;
using UnityCommander.Modules.BottomPanel.Console;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public sealed class ConsoleViewModel : BindableBase
    {
        private readonly ConsoleInputProcessor _inputProcessor;
        private readonly ConsoleAutocompleteProcessor _completeProcessor;
        private readonly ConsoleSession _session;

        public ReadOnlyObservableCollection<CompletionItem> Completions { get; }

        private readonly ObservableCollection<string> _lines = new();
        public ReadOnlyObservableCollection<string> Lines { get; }

        public ConsoleViewModel(IConsoleManager manager)
        {
            _session = manager.Create();
            _inputProcessor = _session.InputProcessor;
            _completeProcessor = _session.CompleteProcessor;

            Completions = new ReadOnlyObservableCollection<CompletionItem>(_session.State.Completions);
            
            Lines = new ReadOnlyObservableCollection<string>(_lines);

            AcceptCommand = new DelegateCommand(Accept, CanAccept)
                .ObservesProperty(() => SelectedIndex);

            CancelCommand = new DelegateCommand(ClearCompletions);

            NavigateUpCommand = new DelegateCommand(NavigateUp);

            NavigateDownCommand = new DelegateCommand(NavigateDown);

            _session.Output.TextWritten += AppendLine;
            _session.Output.Cleared += Clear;
            _session.State.PropertyChanged += OnStatePropertyChanged;

            SendCommand = new DelegateCommand(SendInput);
        }

        public ICommand NavigateUpCommand { get; }
        public ICommand NavigateDownCommand { get; }
        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }

        public string InputText
        {
            get => _session.State.InputText;
            set => _session.State.InputText = value;
        }

        public int CaretIndex
        {
            get => _session.State.CaretIndex;
            set => _session.State.CaretIndex = value;
        }

        public int SelectedIndex
        {
            get => _session.State.SelectedIndex;
            set => _session.State.SelectedIndex = value;
        }

        public DelegateCommand SendCommand { get; }

        public DelegateCommand CopyCommand => new DelegateCommand(() =>
        {
            var text = string.Join(Environment.NewLine, Lines);
            Clipboard.SetText(text);
        });

        private void Clear()
        {
            Application.Current.Dispatcher.Invoke(() => _lines.Clear());
        }

        private void AppendLine(string text)
        {
            Application.Current.Dispatcher.Invoke(() => _lines.Add(text));
        }

        private void SendInput()
        {
            _inputProcessor.SendInput(_session);
        }

        private void ClearCompletions()
        {
            _completeProcessor.ClearCompletions(_session.State);
        }

        private bool CanAccept()
        {
            return _completeProcessor.CanAccept(_session.State);
        }

        private void Accept()
        {
            _completeProcessor.Accept(_session.State);
        }

        private void NavigateDown()
        {
            _inputProcessor.NavigateDown(_session.State);
            //_completeProcessor.ClearCompletions(_session.State);
        }

        private void NavigateUp()
        {
            _inputProcessor.NavigateUp(_session.State);
            //_completeProcessor.ClearCompletions(_session.State);
        }

        private void OnStatePropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConsoleState.InputText))
                RaisePropertyChanged(nameof(InputText));

            if (e.PropertyName == nameof(ConsoleState.CaretIndex))
                RaisePropertyChanged(nameof(CaretIndex));

            if (e.PropertyName == nameof(ConsoleState.SelectedIndex))
                RaisePropertyChanged(nameof(SelectedIndex));
        }
    }
}
