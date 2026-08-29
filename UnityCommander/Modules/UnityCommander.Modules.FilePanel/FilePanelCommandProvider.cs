
using CommandSystem.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityCommander.Modules.FilePanel.States.Resolver;
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

        public Task<UndoToken> ExecuteDeleteAsync(CommandContext ctx)
        {
            var contextMenu = (FilePanelContextMenu)ctx.Context;

            foreach (var path in contextMenu.SelectedFiles)
            {
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                if (!File.Exists(path))
                {
                    Debug.WriteLine(
                        $"File already does not exist: '{path}'");

                    continue;
                }

                try
                {
                    var backup = Path.GetTempFileName();
                    //File.Copy(path, backup, overwrite: true);
                    File.Delete(path);

                    //_notifier.NotifyChanged(path);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error deleting file '{path}': {e.Message}");
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
