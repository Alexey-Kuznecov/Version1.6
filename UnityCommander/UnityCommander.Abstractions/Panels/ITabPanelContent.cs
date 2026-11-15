
namespace UnityCommander.Abstractions.Panels
{
    using System;
    using UnityCommander.Abstractions.Module;

    public interface ITabPanelContent : IViewAttachAware, IDisposable
    {
        event Action<string> PathChanged;
        event Action<string> TabTitleChanged;
        Guid GetTabToken();
        ITabPanelContent InitializedViewModel(
            ref Guid token, 
            string path, 
            Guid panelId, 
            ViewMode viewMode);

        string GetCurrentPath();
        string GetCurrentFilePath();
        void SetCurrentPath(string value);
    }
}
