
using System.Windows.Input;

namespace UnityCommander.Modules.StatusBar.Services
{
    public interface IStatusBarItem
    {
        string Id { get; }

        string OwnerId { get; }

        string Title { get; }

        object Icon { get; }

        string? Description { get; }

        bool IsVisible { get; }

        ICommand? ClickCommand { get; }

        object? Details { get; }
    }
}
