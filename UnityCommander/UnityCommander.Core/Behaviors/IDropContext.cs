
using System.Windows;

namespace UnityCommander.Core.Behaviors
{
    public interface IDropContext
    {
        public object? Data { get; init; }

        public object? Source { get; init; }

        public object? Target { get; init; }

        public UIElement? VisualTarget { get; init; }
    }
}
