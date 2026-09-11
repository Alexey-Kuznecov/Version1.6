
using CommandSystem.Abstractions;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Clipboard;
using UnityCommander.Abstractions.IO;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Abstractions.Selection;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Selection;
using UnityCommander.Core.Clipboard;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel
{
    public class FilePanelCommandProvider
    {
        private readonly IDockingService _dockingService;

        public FilePanelCommandProvider(IDockingService dockingService)
        {
            _dockingService = dockingService;
        }

        public Task GetCurrentPath(CommandContext ctx)
        {
            ctx.Result = _dockingService.GetActiveTabPath();
            return Task.CompletedTask;
        }

        public Task SetCurrentPath(CommandContext ctx)
        {
            var value = ctx.Parameter?.ToString();
            _dockingService.GetActiveDirectoryPanel()?.SetCurrentPath(value);
            return Task.CompletedTask;
        }

        public Task ExecuteGoUpAsync(CommandContext context)
        {
            var selectionService = context.GetService<ISelectionService>();
            var nav = context.GetService<ActiveNavigationService>();
            var manager = selectionService.GetActive();
            
            nav.GoParent();

            manager.RequestSelectFirst();

            return Task.CompletedTask;
        }

        public Task ExecuteRenameAsync(CommandContext ctx)
        {
            var contextMenu = (FilePanelContextMenu)ctx.Context;

            if (contextMenu == null)
            {
                var renameManager = ctx.GetService<IRenameManager>();

                renameManager.Start();

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public Task<UndoToken> ExecuteDeleteAsync(
            CommandContext ctx)
        {
            return Task.Run(() =>
            {
                var contextMenu = ctx.Context as FilePanelContextMenu;

                if (contextMenu == null)
                {
                    var selectionService = ctx.GetService<ISelectionService>();
                    var active = selectionService.GetActive();

                    foreach (var item in active.SelectedItems)
                    {
                        if (item is FolderModel directory)
                        {
                            var path = directory.Path;

                            if (Directory.Exists(path))
                                Directory.Delete(path, recursive: true);
                        }
                        else if (item is FileModel file)
                        {
                            var path = file.Path;

                            if (File.Exists(path))
                                File.Delete(path);
                        }
                    }
                }
                else
                {
                    foreach (var path in contextMenu.SelectedPaths)
                    {
                        if (string.IsNullOrWhiteSpace(path))
                            continue;

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        else if (Directory.Exists(path))
                        {
                            Directory.Delete(path, recursive: true);
                        }
                        else
                        {
                            Debug.WriteLine(
                                $"Path already does not exist: '{path}'");
                        }
                    }
                }

                return (UndoToken)null;
            });
        }

        public Task ExecuteCopyAsync(CommandContext context)
        {
            var selectionService = context.GetService<ISelectionService>();
            var clipboard = context.GetService<FileClipboard>();
            var tabContextAccessor = context.GetService<ITabContextAccessor>();

            if (selectionService is null || clipboard is null)
                return Task.CompletedTask;

            var selected = selectionService.GetActive().SelectedItems;

            clipboard.Set(
                selected.Select(x => ((IDirectoryItem)x).Path),
                FileClipboardOperation.Copy);

            return Task.CompletedTask;
        }

        public Task ExecuteCutAsync(CommandContext context)
        {
            var selectionService = context.GetService<ISelectionService>();
            var clipboard = context.GetService<FileClipboard>();

            if (selectionService is null || clipboard is null)
                return Task.CompletedTask;

            var selected = selectionService.GetActive().SelectedItems;

            foreach ( var item in selected )
            {
                if (item is ICuttableItem cuttable)
                {
                    cuttable.IsCut = true;
                }
            }

            clipboard.Set(
                selected.Select(x => ((IDirectoryItem)x).Path),
                FileClipboardOperation.Cut);

            return Task.CompletedTask;
        }

        public Task ExecutePasteAsync(CommandContext context)
        {
            var clipboard = context.GetService<FileClipboard>();
            var tabContext = context.GetService<ITabContextAccessor>();
            var overrideResolver = context.GetService<ServiceOverrideResolver>();

            if (clipboard is null || !clipboard.HasItems)
                return Task.CompletedTask;

            var fileOperationService =
                overrideResolver.Resolve<IFileOperationService>();

            var type = clipboard.Operation == FileClipboardOperation.Copy
                ? FileOperationType.Copy
                : FileOperationType.Move;

            fileOperationService.CopyAsync(new FileOperationRequest
            {
                ShowDialog = true,
                Target = tabContext.CurrentPath,
                Sources = clipboard.Paths.ToList(),
                Type = type
            });

            return Task.CompletedTask;
        }

        public Task ExecuteOpenFoldersAsync(CommandContext context)
        {
            var selectionService = context.GetService<ISelectionService>();
            var navigation = context.GetService<ActiveNavigationService>();

            if (selectionService is null || navigation is null)
                return Task.CompletedTask;

            var manager = selectionService.GetActive();

            var selected = manager.SelectedItems;

            if (selected.Count != 1)
                return Task.CompletedTask;

            if (selected.FirstOrDefault() is not IFolderItem folder)
                return Task.CompletedTask;

            navigation.Navigate(folder.Path);

            manager.RequestSelectFirst();

            return Task.CompletedTask;
        }

        public Task ExecuteSelectFoldersAsync(CommandContext context)
        {
            var selectionService = context.GetService<ISelectionService>();
            var tabContextAccessor = context.GetService<ITabContextAccessor>();

            var items = tabContextAccessor.ActiveTab
                .GetCurrentDirectoryDirectories();

            SelectAll(items, selectionService);

            return Task.CompletedTask;
        }

        public Task ExecuteSelectFilesAsync(CommandContext context)
        {
            var selectionService = context.GetService<ISelectionService>();
            var tabContextAccessor = context.GetService<ITabContextAccessor>();

            var items = tabContextAccessor.ActiveTab
                .GetCurrentDirectoryFiles();

            SelectAll(items, selectionService);

            return Task.CompletedTask;
        }

        public Task ExecuteSelectAllAsync(CommandContext context)
        {
            var selectionService = context.GetService<ISelectionService>();
            var tabContextAccessor = context.GetService<ITabContextAccessor>();

            var items = tabContextAccessor.ActiveTab
                .GetCurrentDirectoryItems();

            SelectAll(items, selectionService);

            return Task.CompletedTask;
        }

        private void SelectAll(
        IEnumerable<IDirectoryItem> items,
        ISelectionService selectionService)
        {
            var manager = selectionService.GetActive();

            manager.SetItems(items.Cast<ISelectableItem>());

            manager.Handle(new SelectionAction
            {
                Type = SelectionActionType.SelectAll
            });
        }
    }
}
