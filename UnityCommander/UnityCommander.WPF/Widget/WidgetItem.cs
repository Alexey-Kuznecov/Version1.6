
using CommandSystem.Gui.MVVM;
using System.Windows;
using System.Windows.Input;

namespace UnityCommander.WPF.Widget
{
    public sealed class WidgetItem : ObservableObject
    {
        private bool _isExpanded = true;

        public string Id { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public FrameworkElement View { get; init; } = null!;

        public bool IsVisible { get; set; } = true;

        public int Order { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public ICommand ToggleExpandedCommand { get; }

        public WidgetItem()
        {
            ToggleExpandedCommand = new RelayCommand(
                obj => IsExpanded = !IsExpanded);
        }
    }
}
