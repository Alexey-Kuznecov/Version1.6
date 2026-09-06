
using CommandSystem.Abstractions;
using CommandSystem.Core.Execution;
using CommandSystem.Gui.MVVM;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.Controls;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Services.Interfaces;
using UnityCommander.UI.Overlay;
using UnityCommander.UI.Visual;

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

        public Task<UndoToken> ExecuteDeleteAsync(CommandContext ctx)
        {
            var contextMenu = (FilePanelContextMenu)ctx.Context;

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

                        continue;
                    }

                    //try
                    //{
                    //    var backup = Path.GetTempFileName();
                    //    //File.Copy(path, backup, overwrite: true);
                    //    File.Delete(path);

                    //    //_notifier.NotifyChanged(path);
                    //}
                    //catch (Exception e)
                    //{
                    //    Debug.WriteLine($"Error deleting file '{path}': {e.Message}");
                    //}
                }
            }

            return Task.FromResult<UndoToken>(null);
        }

        private Task UndoDeleteAsync(string backup, string path)
        {
            File.Copy(backup, path, overwrite: true);

            //_notifier.NotifyChanged(path);
            return Task.CompletedTask;
        }

        private Task RedoDeleteAsync(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            //_notifier.NotifyChanged(path);
            return Task.CompletedTask;
        }
    }
}
