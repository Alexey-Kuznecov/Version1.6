
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace UnityCommander.Common.Commands
{
    public class UICommand : BindableBase
    {
        public string Id { get; init; }

        public ICommand? Command { get; init; }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.SetProperty(ref _isEnabled, value);
        }

        public object? CommandParameter { get; init; }

        public string Title { get; init; }

        public string Description { get; init; }

        public string IconKey { get; init; }

        public Func<bool>? CanExecute { get; init; }

        public bool IsVisible { get; set; }

        public void RefreshCanExecute()
        {
            IsEnabled = CanExecute?.Invoke() ?? true;
        }
    }
}
