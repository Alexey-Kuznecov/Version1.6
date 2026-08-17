
using CommandSystem.Abstractions;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityCommander.Common.Commands;
using UnityCommander.Core;
using UnityCommander.Core.Commands;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Services;

namespace UnityCommander.Modules.FilePanel
{
    [ModuleDependency(nameof(FilePanelModule))]
    public class FilePanelCommandModule : IModule
    {
        private readonly IContainerProvider _container;
        private IDirectoryChangeNotifier _notifier;

        public FilePanelCommandModule(IContainerProvider container)
        {
            _container = container;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Здесь можно зарегистрировать зависимости модуля, если нужно
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var commandRegistry = containerProvider.Resolve<CommandRegistryService>();
            var filePanelProvider = containerProvider.Resolve<FilePanelCommandProvider>();
            _notifier = containerProvider.Resolve<IDirectoryChangeNotifier>();

            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.Panel.GetCurrentPath,
                filePanelProvider.GetCurrentPath
            ));

            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.Panel.SetCurrentPath,
                filePanelProvider.SetCurrentPath
            ));

            commandRegistry.RegisterUndoable(
                CommandFactoryExtensions.Create(CommandNames.File.Delete, null, ExecuteDeleteAsync,
                contextTypes: typeof(FilePanelContextMenu)));
        }

        private Task<UndoToken> ExecuteDeleteAsync(CommandContext ctx)
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
