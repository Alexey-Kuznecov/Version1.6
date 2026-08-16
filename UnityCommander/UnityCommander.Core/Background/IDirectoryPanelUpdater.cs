
using System;
using UnityCommander.SystemMetrics.Monitoring;

namespace UnityCommander.Core.Background
{
    public interface IDirectoryPanelUpdater
    {
        void Created(Guid tabId, string path, FileSystemEntryType entryType);

        void Deleted(Guid tabId, string path, FileSystemEntryType entryType);

        void Changed(Guid tabId, string path, FileSystemEntryType entryType);

        void Renamed(Guid tabId, string oldPath, string newPath, FileSystemEntryType entryType);
    }
}
