
using System.Windows.Input;

namespace UnityCommander.WPF.Widget
{
    public sealed class WidgetAction
    {
        public string Id { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public string? IconKey { get; init; }

        public ICommand Command { get; init; } = null!;

        public bool IsEnabled { get; init; } = true;
    }
}
