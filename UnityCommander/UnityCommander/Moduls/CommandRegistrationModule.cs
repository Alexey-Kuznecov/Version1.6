
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
            var tooBarProvider = containerProvider.Resolve<ToolCommandProvider>();

            _notifier = containerProvider.Resolve<IDirectoryChangeNotifier>();

            // -------------------------------
            // 1. Регистрация команд файловой панели
            // -------------------------------
            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.Panel.GetCurrentPath,
                filePanelProvider.GetCurrentPath
            ));

            commandRegistry.Register(CommandFactoryExtensions.Create(
                CommandNames.Panel.SetCurrentPath,
                filePanelProvider.SetCurrentPath
            ));

            commandRegistry.RegisterUndoable(CommandFactoryExtensions.Create(
                CommandNames.File.Delete,
                null,
                filePanelProvider.ExecuteDeleteAsync,
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