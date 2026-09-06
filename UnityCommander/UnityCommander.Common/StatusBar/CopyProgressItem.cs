
using Prism.Mvvm;
using System.Windows.Input;
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Common.StatusBar
{
    public class CopyProgressItem : BindableBase, IStatusBarItem
    {
        public string Id { get; }

        public string OwnerId { get; }

        public bool IsVisible { get; set; }

        public string Title => "Copy";

        public string Description =>
            $"{Progress:0}% • {Speed / 1024 / 1024} MB/s";

        private double _progress;

        public double Progress
        {
            get => _progress;
            set
            {
                if (SetProperty(ref _progress, value))
                {
                    RaisePropertyChanged(nameof(Description));

                    if (Progress == 100)
                    {
                        Progress = 0;
                    }
                }
            }
        }

        public bool ShowProgress { get; set; } = true;

        public long Speed { get; set; }

        public object? Icon => "ContentCopy";

        public ICommand? Command { get; set; }

        public object Details { get; set; }
    }
}
