using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Logging.Abstractions;
using UnityCommander.Mvvm;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class LogViewModel : BindableBase
    {
        private readonly StringBuilder _builder = new();
        private ILogHighlighter _highlighter = new DefaultLogHighlighter();
        public ObservableCollection<LogEntryViewModel> Logs { get; } = new();
        
        private string _logText = "";
        public string LogText
        {
            get => _logText;
            set => SetProperty(ref _logText, value);
        }

        public LogViewModel(LogHub hub)
        {
            hub.LogReceived += OnLog;
        }
        public string EditModeText => IsHighlightMode ? "Highlight" : "Edit";

        //private string _editModeText;
        //public string EditModeText
        //{
        //    get => _editModeText;
        //    set => SetProperty(ref _editModeText, value);
        //}

        private bool _isHighlightMode = true;
        public bool IsHighlightMode
        {
            get => _isHighlightMode;
            set => this.SetProperty(ref this._isHighlightMode, value);
        }
        public ICommand ToggleEditModeCommand => new RelayCommand(() =>
        {
            //EditModeText = IsHighlightMode ? "Highlight" : "Edit";
            IsHighlightMode = !IsHighlightMode;
        });

        private void OnLog(LogEntry entry)
        {
            if (entry.Channel != LogChannel.Journal)
                return;
            
            var formatted = FormatEntry(entry);

            Application.Current.Dispatcher.Invoke(() =>
            {
                _builder.AppendLine(formatted);
                LogText = _builder.ToString();

                var vm = new LogEntryViewModel(entry, _highlighter);
                Logs.Add(vm);
            });
        }

        private string FormatEntry(LogEntry e)
        {
            return
            $"[{e.Scope}] " +
            $"[{e.Category}] " +
            $"[{e.Level}] " +
            $"{(string.IsNullOrWhiteSpace(e.Source) ? "" : $"[{e.Source}] ")}" +
            $"{e.Message}";
        }
    }
}
