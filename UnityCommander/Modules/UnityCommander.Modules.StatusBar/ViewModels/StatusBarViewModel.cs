
using System.Collections.ObjectModel;
using UnityCommander.Modules.StatusBar.Services;
using UnityCommander.Modules.StatusBar.Views;
using UnityCommander.Mvvm.Base;
using UnityCommander.Services.Background;
using IViewRegistry = UnityCommander.Core.Registrar.IViewRegistry;

namespace UnityCommander.Modules.StatusBar.ViewModels
{
    public class StatusBarViewModel : PropertiesChanged
    {
        public ObservableCollection<IStatusBarItem> Items { get; set; }
            = new ObservableCollection<IStatusBarItem>();

        public StatusBarViewModel(BackgroundServiceHost background, IViewRegistry viewRegistry)
        {
            viewRegistry.Register<CopyProgressViewModel, CopyProgressView>();
            viewRegistry.Register<WatchDirectoryViewModel, WatchDirectoryView>();

            foreach (var item in background.GetItems())
            {
                Items.Add(item);
            }
        }
    }
}
