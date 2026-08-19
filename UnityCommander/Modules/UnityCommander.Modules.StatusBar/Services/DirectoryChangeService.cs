
using System.IO;
using System.Windows;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.StatusBar;
using UnityCommander.Modules.StatusBar.Services;
using UnityCommander.Modules.StatusBar.ViewModels;
using UnityCommander.Services.Interfaces;
using UnityCommander.SystemMetrics.Monitoring;
using UnityCommander.WPF;

namespace UnityCommander.Core.Background
{
    public class DirectoryChangeService : IBackgroundService, IStatusBarProvider
    {
        private readonly IDirectoryWatchManager _watchManager;
        private readonly IDirectoryPanelUpdater _updater;
        private readonly IPanelRegistry _panelRegistry;
        private readonly ITabRegistry _tabRegistry;
        private IDirectoryChangeNotifier _notifier;

        private readonly IVisibleTabResolver _visibleTabResolver;
        private readonly WatchDirectoryItem _item;

        public DirectoryChangeService(
            IDirectoryWatchManager watchManager,
            IDirectoryPanelUpdater updater,
            ITabRegistry tabRegistry, 
            IPanelRegistry panelRegistry,
            IPopupService popup,
            IDirectoryChangeNotifier notifier,
            IVisibleTabResolver visibleTabResolver)
        {
            _item = new WatchDirectoryItem();
            _item.Details = new WatchDirectoryViewModel();
            _item.Command = new DelegateCommand<FrameworkElement>(obj =>
            {
                popup.Show(obj, _item.Details);
            });

            _tabRegistry = tabRegistry;
            _panelRegistry = panelRegistry;
            _notifier = notifier;
            _watchManager = watchManager;
            _updater = updater;
            _visibleTabResolver = visibleTabResolver;
        }

        public string Id => "core.directory.change.service";

        public string Name => "Directory Change Service";

        public bool IsRunning { get; private set; }

        public bool AutoStart => true;

        public string OwnerId => "core.backround.service";

        private void OnActiveTabChanged(ActiveTabChangedEvent obj)
        {
            SynchronizeVisibleTabs();

            var tab = _tabRegistry.GetTab(obj.TabId);

            if (tab == null)
                return;

            //_notifier.NotifyChanged(tab.GetCurrentPath());
        }

        private void SynchronizeVisibleTabs()
        {
            var visibleTabIds = _visibleTabResolver
                .GetVisibleTabs()
                .ToHashSet();

            // Добавляем watcher только для реально видимых вкладок
            foreach (var tabId in visibleTabIds)
            {
                var tab = _tabRegistry.GetTab(tabId);

                if (tab == null)
                    continue;

                EnsureWatching(tab);
            }

            // Убираем watcher со всех невидимых вкладок
            foreach (var watchingTabId in _watchManager.GetAll())
            {
                if (visibleTabIds.Contains(watchingTabId.Tag))
                    continue;

                _watchManager.Unwatch(watchingTabId.Tag);
            }
        }

        private bool EnsureWatching(ITabContentAdapter tab)
        {
            if (_watchManager.IsWatching(tab.TabId))
                return true;

            tab.PathChanged += path =>
            {
                _watchManager.Unwatch(tab.TabId);
                _watchManager.Watch(tab.TabId, path);
            };

            _watchManager.Watch(
                tab.TabId,
                tab.GetCurrentPath());

            return false;
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
                    _updater.Created(e.Token, e.FullPath, e.EntryType);
                    break;

                case WatcherChangeTypes.Deleted:
                    _updater.Deleted(e.Token, e.FullPath, e.EntryType);
                    break;

                case WatcherChangeTypes.Changed:
                    _updater.Changed(e.Token, e.FullPath, e.EntryType);
                    break;

                case WatcherChangeTypes.Renamed:
                    _updater.Renamed(e.Token, e.OldPath, e.FullPath, e.EntryType);
                    break;
            }
        }

        public Task RunAsync(CancellationToken token)
        {
            IsRunning = true;

            _watchManager.FileChanged += OnFileChanged;

            _panelRegistry.TabAdded += OnTabAdded;
            _panelRegistry.TabRemoved += OnTabRemoved;
            _panelRegistry.ActiveTabChanged += OnActiveTabChanged;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            IsRunning = false;

            _watchManager.FileChanged -= OnFileChanged;

            _panelRegistry.TabAdded -= OnTabAdded;
            _panelRegistry.TabRemoved -= OnTabRemoved;
            _panelRegistry.ActiveTabChanged -= OnActiveTabChanged;

            return Task.CompletedTask;
        }

        public IEnumerable<IStatusBarItem> GetItems()
        {
            yield return _item;
        }
    }
}
