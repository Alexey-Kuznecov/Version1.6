using System.Windows;
using UnityCommander.Core.Behaviors;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class FilePanelDragDropContext
     : IDropContext
    {
        public object? Data { get; init; }

        public object? Source { get; init; }

        public object? Target { get; init; }

        public UIElement? VisualTarget { get; init; }
    }
}
