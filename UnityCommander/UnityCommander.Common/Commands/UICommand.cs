
using Prism.Mvvm;
using System;
using System.Windows.Input;
using UnityCommander.Common.Models.Icons;

namespace UnityCommander.Common.Commands
{
    public class UICommand : BindableBase
    {
        public string Id { get; init; }

        public ICommand Command { get; init; }

        public string Title { get; init; }

        public string Description { get; init; }

        public IIcon Icon { get; init; }

        public Func<bool>? CanExecute { get; init; }

        public bool IsEnabled => CanExecute?.Invoke() ?? true;

        public bool IsVisible { get; set; }
    }
}
