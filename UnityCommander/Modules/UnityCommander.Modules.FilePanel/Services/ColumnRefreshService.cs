
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.IO;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Converters;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services.Interfaces;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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

        private bool _isUnified = true;

        private readonly IFileActivityService _activityService;

        private readonly IVisibleTabResolver _visibleTabResolver;

        public ColumnRefreshService(
            IColumnRegistry registry,
            NodeContextRegistry contexts,
            IVisibleTabResolver visibleTabResolver, 
            IFileActivityService activityService)
        {
            _activityService = activityService;
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

                    //foreach (var ctx in _contexts.FileContexts)
                    //{
                    //    if (!visibleTabs.Contains(ctx.TabId))
                    //        continue;

                    //    if (ctx.IsUnified)
                    //    {
                    //        _isUnified = true;
                    //        continue;
                    //    } 

                    //    await RefreshFilesAsync(ctx);
                    //}

                    //foreach (var ctx in _contexts.FolderContexts)
                    //{
                    //    if (!visibleTabs.Contains(ctx.TabId))
                    //        continue;

                    //    if (ctx.IsUnified)
                    //    {
                    //        _isUnified = true;
                    //        continue;
                    //    } 


                    //    await RefreshFoldersAsync(ctx);
                    //}

                    foreach (var ctx in _contexts.UnifiedContexts)
                    {
                        if (!visibleTabs.Contains(ctx.TabId))
                            continue;

                        //if (!ctx.IsUnified)
                        //{
                        //    continue;
                        //}

                        await RefreshUnifiedAsync(ctx);
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

            var columnIds = columns
              .Select(x => x.Id)
              .ToHashSet();

           

            foreach (var file in ctx.ScrollService?.GetVisibleItems())
            {
                var now = DateTime.UtcNow; 
                foreach (var key in file.Cell.Additional.Keys.ToList())
                {
                    if (!columnIds.Contains(key))
                        file.Cell.Additional.Remove(key);
                }

                for (var index = 0; index < columns.Count; index++)
                {
                    var column = columns[index];

                    file.LastUpdate.TryGetValue(column.Id, out var last);

                    var interval = GetInterval(column, file);

                    if ((now - last).TotalMilliseconds < interval)
                        continue;

                    var newValue = column.ColumnValueHandler(file);

                    file.LastUpdate[column.Id] = now;

                    //file.Cell.CurrentColumnId = column.Id;

                    if (!Equals(file.Cell.Additional.GetValueOrDefault(column.Id), newValue))
                    {
                        file.Cell.Additional[column.Id] = newValue;
                    }

                    file.Cell.OutputOrder = 0;
                }
            }

            return Task.CompletedTask;
        }

#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        private IDiagnosticTrace? _trace = ContainerLocator.Container.Resolve<IDiagnosticTrace>();
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".

        private Task RefreshUnifiedAsync(UnifiedNodeContext ctx)
        {
            //using var trace = _trace.Begin(
            //  "DynamicColumns",
            //  "RefreshUnified",
            //  new Dictionary<string, object?>
            //  {
            //      ["TabId"] = ctx.TabId,
            //      ["IsUnified"] = ctx.IsUnified,
            //      ["ItemCount"] = ctx.Items.Count
            //  });

            var columns = _registry
                .GetColumns(PanelType.Unified)
                .Where(x => x.IsDynamic)
                .OrderBy(x => x.Order)
                .ToList();

            //trace.Write(
            // "ColumnsResolved",
            // new Dictionary<string, object?>
            // {
            //     ["Count"] = columns.Count,
            //     ["Columns"] = string.Join(
            //         ", ",
            //         columns.Select(x => $"{x.Id}[Order={x.Order}]"))
            // });

            var columnIds = columns
               .Select(x => x.Id)
               .ToHashSet();
             

            foreach (var item in ctx.ScrollService?.GetVisibleItems() ?? [])
            {
                var now = DateTime.UtcNow;

                foreach (var key in item.Cell.Additional.Keys.ToList())
                {
                    if (!columnIds.Contains(key))
                        item.Cell.Additional.Remove(key);
                }

                for (var index = 0; index < columns.Count; index++)
                {
                    var column = columns[index];

                    item.LastUpdate.TryGetValue(column.Id, out var last);

                    var interval = GetInterval(column, item);

                    if ((now - last).TotalMilliseconds < interval)
                        continue;

                    var newValue = column.ColumnValueHandler(item);

                    item.LastUpdate[column.Id] = now;

                    var oldValue = item.Cell.Additional.GetValueOrDefault(column.Id);

                    if (!Equals(oldValue, newValue))
                    {
                        item.Cell.Additional[column.Id] = newValue;
                    }

                    item.Cell.OutputOrder = 0;
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

            var columnIds = columns
                .Select(x => x.Id)
                .ToHashSet();
             

            foreach (var folder in ctx.ScrollService?.GetVisibleItems() ?? [])
            {
                var now = DateTime.UtcNow;

                foreach (var key in folder.Cell.Additional.Keys.ToList())
                {
                    if (!columnIds.Contains(key))
                        folder.Cell.Additional.Remove(key);
                }

                for (var index = 0; index < columns.Count; index++)
                {
                    var column = columns[index];

                    folder.LastUpdate.TryGetValue(column.Id, out var last);

                    var interval = GetInterval(column, folder);

                    if ((now - last).TotalMilliseconds < interval)
                        continue;

                    var newValue = column.ColumnValueHandler(folder);

                    folder.LastUpdate[column.Id] = now;

                    //folder.Cell.CurrentColumnId = column.Id;

                    if (!Equals(
                            folder.Cell.Additional.GetValueOrDefault(column.Id),
                            newValue))
                    {
                        folder.Cell.Additional[column.Id] = newValue;
                    }

                    folder.Cell.OutputOrder = 0;
                }
            }

            return Task.CompletedTask;
        }

        private int GetInterval(
           ColumnModel column,
           IDirectoryItem folder)
        {
            if (_activityService.IsActive(folder.Path))
                return 500;

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
