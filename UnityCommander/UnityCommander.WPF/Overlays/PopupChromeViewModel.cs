
using System.Windows;
using System.Windows.Input;
using UnityCommander.Mvvm.Base;

namespace UnityCommander.WPF.Overlays
{
    public sealed class PopupChromeViewModel : PropertiesChanged
    {
        private readonly Action _close;
        private readonly Action<bool> _setPinned;

        private bool _isPinned;

        public object Content { get; }

        public string? Title { get; }

        public bool IsPinnable { get; }

        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (!SetProperty(ref _isPinned, value))
                    return;
            }
        }

        public string PinIconKey =>
            IsPinned ? "Pinned" : "Pin";

        public ICommand TogglePinCommand { get; }

        public ICommand CloseCommand { get; }

        public Visibility IsPinnableVisibility =>
            IsPinnable
                ? Visibility.Visible
                : Visibility.Collapsed;

        public PopupChromeViewModel(
            object content,
            string? title,
            bool isPinnable,
            Action close,
            Action<bool> setPinned)
        {
            Content = content;
            Title = title;
            IsPinnable = isPinnable;

            _close = close;
            _setPinned = setPinned;

            TogglePinCommand = new DelegateCommand(TogglePin);
            CloseCommand = new DelegateCommand(_close);
        }

        private void TogglePin()
        {
            IsPinned = !IsPinned;
            _setPinned(IsPinned);

            OnPropertyChanged(nameof(PinIconKey));
            OnPropertyChanged(nameof(IsPinnableVisibility));
        }
    }
}
