
using Prism.Mvvm;
using System.Windows.Input;
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Common.StatusBar
{
    public class WatchDirectoryItem : BindableBase, IStatusBarItem
    {
        public string Id { get; }

        public string OwnerId { get; }

        public bool IsVisible { get; set; }

        public string Title => "Watch";

        public string Description {  get; set; }
          
        public long Speed { get; set; }

        public object? Icon => "DatabaseSync";

        public ICommand? Command { get; set; }

        public object Details { get; set; }

        public bool ShowProgress { get; set; } = false;

        public double Progress { get; set; } 
    }
}
