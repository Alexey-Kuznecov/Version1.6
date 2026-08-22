
using System.Windows;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class FilePanelDragDropContext
     : IDropContext
    {
        public object? Data { get; init; }

        public object? Source { get; init; }

        public object? Target { get; init; }

        public UIElement? VisualTarget { get; init; }
        
        public DropTargetInfo TargetInfo { get; init; }

        public object TabId { get; internal set; }

        public bool CanNavigate { get; init; }
    }
}
