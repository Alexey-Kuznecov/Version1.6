
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Panels;
using UnityCommander.Services.Background;
using UnityCommander.SystemMetrics.Monitoring;

namespace UnityCommander.Services.Interfaces
{
    public class DirectoryChangeService : IBackgroundService
    {
        private readonly IDirectoryWatchManager _watchManager;
        private readonly IDirectoryPanelUpdater _updater;
        private readonly IPanelRegistry _panelRegistry;
        private readonly ITabRegistry _tabRegistry;

        public DirectoryChangeService(
            IDirectoryWatchManager watchManager,
            IDirectoryPanelUpdater updater,
            ITabRegistry tabRegistry,
            IPanelRegistry panelRegistry)
        {
            _tabRegistry = tabRegistry;
            _panelRegistry = panelRegistry;
            _watchManager = watchManager;
            _updater = updater;
        }

        private void OnActiveTabChanged(ActiveTabChangedEvent obj)
        {
            var tab = _tabRegistry.GetTab(obj.TabId);

            if (_watchManager.IsWatching(tab.TabId))
                return;

            tab.PathChanged += (path) =>
            {
                _watchManager.Unwatch(tab.TabId);
                _watchManager.Watch(tab.TabId, path);
            };

            _watchManager.Watch(obj.TabId, tab.GetCurrentPath());
        }

        private void OnTabAdded(TabAddedEvent panel)
        {
            var tab = _tabRegistry.GetTab(panel.TabId);

            _watchManager.Watch(panel.TabId, tab.GetCurrentPath());
        }

        private void OnTabRemoved(TabRemovedEvent panel)
        {
            var tab = _tabRegistry.GetTab(panel.TabId);

            _watchManager.Unwatch(panel.TabId);
        }

        private void OnFileChanged(object sender, FileSystemChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    _updater.FileCreated(e.Token, e.FullPath);
                    break;

                case WatcherChangeTypes.Deleted:
                    _updater.FileDeleted(e.Token, e.FullPath);
                    break;

                case WatcherChangeTypes.Changed:
                    _updater.FileChanged(e.Token, e.FullPath);
                    break;
            }
        }

        public Task RunAsync(CancellationToken token)
        {
            _watchManager.FileChanged += OnFileChanged;

            _panelRegistry.TabAdded += OnTabAdded;
            _panelRegistry.TabRemoved += OnTabRemoved;
            _panelRegistry.ActiveTabChanged += OnActiveTabChanged;

            return Task.CompletedTask;
        }
    }
}
