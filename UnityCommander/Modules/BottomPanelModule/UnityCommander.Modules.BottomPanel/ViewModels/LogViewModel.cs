using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public class LogViewModel : BindableBase
    {
        private readonly IAppLogger logger;

        private StringBuilder logBuilder = new();

        private string _logText = string.Empty;
        public string LogText
        {
            get => _logText;
            set => SetProperty(ref _logText, value);
        }

        public LogViewModel(IAppLogger logger)
        {
            this.logger = logger;

            logger.OnLog += e =>
            {
                var formatted = FormatEntry(e);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    logBuilder.AppendLine(formatted);
                    LogText = logBuilder.ToString();
                });
            };
        }

        private string FormatEntry(LogEntry e)
        {
            return
                $"[{e.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] " +
                $"[{e.Level}] " +
                $"{(string.IsNullOrWhiteSpace(e.Source) ? "" : $"[{e.Source}] ")}" +
                $"{e.Message}";
        }
    }
}
