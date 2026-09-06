
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

        ICommand? Command { get; }

        object? Details { get; }

        bool ShowProgress { get; set; }

        double Progress { get; set; }
    }
}
