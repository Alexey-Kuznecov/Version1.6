
using System;

namespace UnityCommander.Services.Background
{
    public interface IDirectoryPanelUpdater
    {
        void FileCreated(Guid tabId, string path);

        void FileDeleted(Guid tabId, string path);

        void FileChanged(Guid tabId, string path);

        void FileRenamed(Guid tabId, string oldPath, string newPath);
    }
}
