
namespace UnityCommander.Abstractions.Panels
{
    using System;
    using UnityCommander.Abstractions.Module;

    public interface ITabPanelContent : IViewAttachAware, IDisposable
    {
        event Action<string> PathChanged;
        event Action<string> TabTitleChanged;
        bool IsActive { get; }
        Guid GetPanelToken();
        ITabPanelContent InitializedViewModel(ref Guid token, string path);
        string GetCurrentPath();
        string GetCurrentFilePath();
        void SetCurrentPath(string value);
    }
}
