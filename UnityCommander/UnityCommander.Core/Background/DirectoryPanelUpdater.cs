
using System;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Panels;
using UnityCommander.SystemMetrics.Monitoring;

namespace UnityCommander.Core.Background
{
    public class DirectoryPanelUpdater : IDirectoryPanelUpdater
    {
        private readonly ITabRegistry _tabRegistry;
        private readonly FileModelFactory _factory;
        private readonly FolderModelFactory _folderFactory;

        public DirectoryPanelUpdater(
            ITabRegistry tabRegistry,
            FileModelFactory factory,
            FolderModelFactory folderFactory)
        {
            _factory = factory;
            _folderFactory = folderFactory;
            _tabRegistry = tabRegistry;
        }

        public void Changed(Guid tabId, string path, FileSystemEntryType type)
        {
            var tab = _tabRegistry.GetTab(tabId);

            switch (type)
            {
                case FileSystemEntryType.File:
                    tab.GetContent().FileContext.Update(_factory.Create(path));
                    break;

                case FileSystemEntryType.Directory:
                    tab.GetContent().FolderContext.Update(_folderFactory.Create(path));
                    break;
            }
        }

        public void Created(Guid tabId, string path, FileSystemEntryType type)
        {
            var tab = _tabRegistry.GetTab(tabId);

            switch (type)
            {
                case FileSystemEntryType.File:
                    tab.GetContent().FileContext.Add(_factory.Create(path));
                    break;

                case FileSystemEntryType.Directory:
                    tab.GetContent().FolderContext.Add(_folderFactory.Create(path));
                    break;
            }
        }

        public void Deleted(Guid tabId, string path, FileSystemEntryType type)
        {
            var tab = _tabRegistry.GetTab(tabId);

            switch (type)
            {
                case FileSystemEntryType.File:
                    tab.GetContent().FileContext.Remove(path);
                    break;

                case FileSystemEntryType.Directory:
                    tab.GetContent().FolderContext.Remove(path);
                    break;
            }
        }

        public void Renamed(Guid tabId, string oldPath, string newPath, FileSystemEntryType type)
        {
            var tab = _tabRegistry.GetTab(tabId);

            switch (type)
            {
                case FileSystemEntryType.File:
                    tab.GetContent().FileContext.Rename(oldPath, newPath);
                    break;

                case FileSystemEntryType.Directory:
                    tab.GetContent().FolderContext.Rename(oldPath, newPath);
                    break;
            }
        }
    }
}
