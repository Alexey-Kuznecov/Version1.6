
using System;

namespace UnityCommander.Core.Navigation
{
    public interface INavigationService
    {
        string Current { get; }
        
        bool CanGoBack { get; }
        
        bool CanGoForward { get; }

        bool IsValidPath(string path);

        bool TryNavigateTo(string path);

        bool TryNavigateTo(string path, bool forceRecord);

        public void GoBack();

        event Action<string> CurrentChanged;
    }
}
