
namespace UnityCommander.Common.Module
{
    using System;

    public interface ITabPanelContent
    {
        event Action<string> PathChanged;
        public event Action<string> TabTitleChanged;
        public bool IsActive { get; }
        public Guid GetPanelToken();
        public ITabPanelContent InitializedViewModel(ref Guid token, string path);
        public string GetCurrentPath();
        public string GetCurrentFilePath();
        void SetCurrentPath(string value);
    }
}
