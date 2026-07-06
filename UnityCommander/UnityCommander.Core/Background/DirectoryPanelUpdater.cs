
using System;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Panels;

namespace UnityCommander.Core.Background
{
    public class DirectoryPanelUpdater : IDirectoryPanelUpdater
    {
        private readonly ITabRegistry _tabRegistry;
        private readonly FileModelFactory _factory;

        public DirectoryPanelUpdater(
            ITabRegistry tabRegistry, 
            FileModelFactory factory)
        {
            _factory = factory;
            _tabRegistry = tabRegistry;
        }

        public void FileChanged(Guid tabId, string path)
        {
            var tab = _tabRegistry.GetTab(tabId);

            var files = tab.GetContent().FileContext;

            files.Update(_factory.Create(path));
        }

        public void FileCreated(Guid tabId, string path)
        {
            var tab = _tabRegistry.GetTab(tabId);

            var files = tab.GetContent().FileContext;

            files.Add(_factory.Create(path));
        }

        public void FileDeleted(Guid tabId, string path)
        {
            var tab = _tabRegistry.GetTab(tabId);

            var files = tab.GetContent().FileContext;

            files.Remove(path);
        }

        public void FileRenamed(Guid tabId, string oldPath, string newPath)
        {
            var tab = _tabRegistry.GetTab(tabId);

            var files = tab.GetContent().FileContext;
        }
    }
}
