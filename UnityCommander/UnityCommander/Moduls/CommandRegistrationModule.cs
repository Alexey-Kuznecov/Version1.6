
using Microsoft.Win32;
using Prism.Ioc;
using Prism.Modularity;
using UnityCommander.Common.Commands;
using UnityCommander.Core;
using UnityCommander.Core.Commands;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Modules.BottomPanel.Commands;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Modules.ToolBar.Commands;
using UnityCommander.Services;

namespace UnityCommander.Moduls
{
    public sealed class CommandRegistrationModule : IModule
    {
        private IDirectoryChangeNotifier _notifier;

        public void OnInitialized(IContainerProvider containerProvider)
        {
         
            var commandRegistry = containerProvider.Resolve<CommandRegistryService>();
            var filePanelProvider = containerProvider.Resolve<FilePanelCommandProvider>();
            var ribbonProvider = containerProvider.Resolve<RobbonCommandProvider>();
            var tooBarProvider = containerProvider.Resolve<ToolCommandProvider>();

            _notifier = containerProvider.Resolve<IDirectoryChangeNotifier>();

            commandRegistry.Register(CommandFactoryExtensions.Create(
               CommandNames.Test.ShowDialog,
                ribbonProvider.ShowDialogTest,
                null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
               CommandNames.Test.ShowParams,
                ribbonProvider.ShowParamTest,
                null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
              CommandNames.Clipboard.Copy,
               filePanelProvider.ExecuteCopyAsync,
               null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
              CommandNames.Clipboard.Cut,
               filePanelProvider.ExecuteCutAsync,
               null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
              CommandNames.Clipboard.Paste,
               filePanelProvider.ExecutePasteAsync,
               null));

            // -------------------------------
            // 1. Регистрация команд файловой панели
            // -------------------------------
            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.Navigation.GoUp,
                filePanelProvider.ExecuteGoUpAsync,
                null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.File.Open,
                filePanelProvider.ExecuteOpenFoldersAsync,
                null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.Directory.SelectAll,
                filePanelProvider.ExecuteSelectFoldersAsync,
                null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
                 CommandNames.File.SelectAll,
                 filePanelProvider.ExecuteSelectFilesAsync,
                 null));

            commandRegistry.Register(CommandFactoryExtensions.Create(
                 CommandNames.Panel.SelectAll,
                 filePanelProvider.ExecuteSelectAllAsync,
                 null));

            commandRegistry.RegisterUndoable(CommandFactoryExtensions.Create(
                CommandNames.File.Delete,
                null,
                filePanelProvider.ExecuteDeleteAsync,
                contextTypes: typeof(FilePanelContextMenu)));

            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.File.Rename,
                filePanelProvider.ExecuteRenameAsync,
                null,
                contextTypes: typeof(FilePanelContextMenu)));

            // -------------------------------
            // 2. Регистрация команд панели интсрументов
            // -------------------------------
            commandRegistry.Register(CommandFactoryExtensions.Create(
               CommandNames.ToolBar.Create,
               tooBarProvider.CreateTool
           ));
        }

        public CommandRegistrationModule(IContainerProvider container)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Здесь можно зарегистрировать зависимости модуля, если нужно
        }

    }
}