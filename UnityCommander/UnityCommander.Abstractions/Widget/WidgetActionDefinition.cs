
using System.Windows.Input;

namespace UnityCommander.Abstractions.Widget
{
    public sealed class WidgetActionDefinition
    {
        public string Id { get; init; } = string.Empty;

        public string Title { get; init; } = string.Empty;

        public string? IconKey { get; init; }

        public Func<object, ICommand> CreateCommand { get; init; } = null!;
    }
}
