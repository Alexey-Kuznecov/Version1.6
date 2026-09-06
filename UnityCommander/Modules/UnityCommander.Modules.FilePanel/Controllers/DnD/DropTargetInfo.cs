
using System;
using System.Windows.Input;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class DropTargetInfo
    {
        public string? Path { get; init; }
        public Guid? TabId { get; init; }
        public bool CanNavigate { get; set; }

        public ICommand? NavigateCommand { get; set; }

        public object? NavigationTarget { get; set; }
    }
}
