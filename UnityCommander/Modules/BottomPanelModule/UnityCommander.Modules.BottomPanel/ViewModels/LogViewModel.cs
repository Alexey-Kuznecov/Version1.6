using Prism.Mvvm;
using System.Text;
using System.Windows;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class LogViewModel : BindableBase
    {
        private readonly StringBuilder _builder = new();

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

        private void OnLog(LogEntry entry)
        {
            if (entry.Channel != LogChannel.Journal)
                return;

            var formatted = FormatEntry(entry);

            Application.Current.Dispatcher.Invoke(() =>
            {
                _builder.AppendLine(formatted);
                LogText = _builder.ToString();
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
