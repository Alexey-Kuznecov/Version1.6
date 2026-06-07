
using System.Collections.Generic;
using System.Windows;

namespace UnityCommander.Core.Behaviors
{
    public sealed class DragDropContext : IDropContext
    {
        public object? Data { get; init; }

        public object? Source { get; init; }

        public object? Target { get; init; }

        public UIElement? VisualTarget { get; init; }
        
        public UIElement? VisualSource { get; init; }

        public object? SourceContext { get; init; }
        
        public object? TargetContext { get; init; }
        
        public string? SourcePath { get; init; }
        
        public string? TargetPath { get; init; }

        public IReadOnlyList<object> SourceItems { get; init; }
    }
}
