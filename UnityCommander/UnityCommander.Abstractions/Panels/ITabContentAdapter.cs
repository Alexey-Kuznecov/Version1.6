
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

        IDirectoryPanel GetContent();
    }
}
