
using AdvancedCopyFiles.Services;
using CommandSystem.CopyTester.ViewModels;
using CommandSystem.Gui.MVVM;
using UnityCommander.Copying;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Progress;

namespace AdvancedCopyFiles.ViewModels
{
    public class CopyWindowViewModel : ObservableObject
    {
        public ProgressViewModel ProgressVM { get; }
        public FileListViewModel FileListVM { get; }
        public SettingsViewModel SettingsVM { get; }
        public LogViewModel LogVM { get; }
        public HistoryViewModel HistoryVM { get; }
        public MetricViewModel MetricVM { get; }

        public SpeedGraphViewModel SpeedGraphVM { get; }

        public CopyWindowViewModel(
            FileListViewModel fileListViewModel,
            ProgressViewModel progressViewModel, 
            SettingsViewModel settingsViewModel, 
            HistoryViewModel historyViewModel, 
            MetricViewModel metricViewModel)
        {
            // Инициализация под-VM
            FileListVM = fileListViewModel;
            ProgressVM = progressViewModel;
            SettingsVM = settingsViewModel;
            HistoryVM = historyViewModel;
            MetricVM = metricViewModel;
        }
    }
}
