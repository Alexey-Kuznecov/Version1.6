
using UnityCommander.Abstractions.Module;

namespace UnityCommander.Abstractions.Panels
{
    public interface ITabContentAdapter : IAttachAware, IDisposable
    {
        event Action<string> PathChanged;
        bool IsActive { get; }
        Guid TabId { get; }
        string GetCurrentPath();

        IReadOnlyList<IDirectoryItem> GetCurrentDirectoryFiles();

        IReadOnlyList<IDirectoryItem> GetCurrentDirectoryItems();

        IReadOnlyList<IDirectoryItem> GetCurrentDirectoryDirectories();

        IDirectoryPanel GetContent();
    }
}
