
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.Services
{
    public sealed class ColumnRefreshService : IBackgroundService
    {
        private readonly NodeContextRegistry _contexts;

        public string Id => "column-refresh-service";

        public string Name => "Column Refresh Service";

        public bool IsRunning { get; private set; }

        public bool AutoStart => true;

        public string OwnerId => "core.background.service";

        private readonly IColumnRegistry _registry;

        private readonly IVisibleTabResolver _visibleTabResolver;

        public ColumnRefreshService(
            IColumnRegistry registry,
            NodeContextRegistry contexts,
            IVisibleTabResolver visibleTabResolver)
        {
            _registry = registry;
            _contexts = contexts;
            _visibleTabResolver = visibleTabResolver;
        }

        public async Task RunAsync(CancellationToken token)
        {
            if (IsRunning)
                return;

            IsRunning = true;

            try
            {
                while (IsRunning && !token.IsCancellationRequested)
                {
                    var visibleTabs = _visibleTabResolver.GetVisibleTabs();

                    foreach (var ctx in _contexts.FileContexts)
                    {
                        if (!visibleTabs.Contains(ctx.TabId))
                            continue;

                        await RefreshFilesAsync(ctx);
                    }

                    foreach (var ctx in _contexts.FolderContexts)
                    {
                        if (!visibleTabs.Contains(ctx.TabId))
                            continue;

                        await RefreshFoldersAsync(ctx);
                    }

                    await Task.Delay(50, token);
                }
            }
            finally
            {
                IsRunning = false;
            }
        }

        private Task RefreshFilesAsync(FileNodeContext ctx)
        {
            var columns = _registry
                .GetColumns(PanelType.Files)
                .Where(x => x.IsDynamic)
                .OrderBy(x => x.Order)
                .ToList();

            foreach (var file in ctx.ScrollService?.GetVisibleItems())
            {
                var now = DateTime.UtcNow;

                foreach (var column in columns)
                {
                    file.LastUpdate.TryGetValue(column.Id, out var last);

                    var interval = GetInterval(column);

                    if ((now - last).TotalMilliseconds < interval)
                        continue;

                    var newValue = column.ColumnValueHandler(file);

                    file.LastUpdate[column.Id] = now;

                    if (!Equals(file.Additional.GetValueOrDefault(column.Id), newValue))
                    {
                        file.Additional[column.Id] = newValue;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task RefreshFoldersAsync(FolderNodeContext ctx)
        {
            var columns = _registry
                .GetColumns(PanelType.Folders)
                .Where(x => x.IsDynamic)
                .ToList();

            foreach (var folder in ctx.ScrollService?.GetVisibleItems())
            {
                var now = DateTime.UtcNow;

                foreach (var column in columns)
                {
                    folder.LastUpdate.TryGetValue(column.Id, out var last);

                    var interval = GetInterval(column);

                    if ((now - last).TotalMilliseconds < interval)
                        continue;

                    var newValue = column.ColumnValueHandler(folder);

                    folder.LastUpdate[column.Id] = now;

                    if (!Equals(folder.Additional.GetValueOrDefault(column.Id), newValue))
                    {
                        folder.Additional[column.Id] = newValue;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private int GetInterval(ColumnModel column)
        {
            if (column.RefreshInterval.HasValue)
                return column.RefreshInterval.Value;

            if (column.UpdatePriority != ColumnUpdatePriority.Ignore)
                return (int)column.UpdatePriority;

            return (int)ColumnUpdatePriority.Normal;
        }

        public Task StopAsync()
        {
            IsRunning = false;

            return Task.CompletedTask;
        }
    }
}
