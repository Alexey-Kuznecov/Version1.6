

using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.CLI.Bootstrap;
using UnityCommander.CLI.Integration;
using UnityCommander.Commands;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Dialog;
using UnityCommander.Core.Commands;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.FilePanel.Dialog;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Modules.FilePanel.Views;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.SettingsPanel.ViewModels;
using UnityCommander.Modules.SettingsPanel.Views;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Services;
using UnityCommander.Services.Background;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Interfaces;
using UnityCommander.Settings.Abstactions;
using UnityCommander.ViewModels.Dialogs;
using UnityCommander.Views.CopyDialogs;
using UnityCommander.Views.Dialogs;
using IViewRegistry = UnityCommander.Core.Registrar.IViewRegistry;

namespace UnityCommander
{
    [ModuleDependency(nameof(FilePanelModule))]
    [ModuleDependency(nameof(LeftSideBarsModule))]
    [ModuleDependency(nameof(ToolBarModule))]
    internal class AppLoadModule : IModule
    {
        private static ILogger _logger;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _logger = Log.Create("AppLoadModule", LogScope.Startup);
    
            RegisterConsoleCommands(containerProvider);
            RegisterShortcuts(containerProvider);
            RegisterDiaglog(containerProvider);
            RegisterCommand(containerProvider);
            RegisterViewModels(containerProvider);

            var initializer = containerProvider.Resolve<AppInitializer>();
            var backgroundService = containerProvider.Resolve<BackgroundServiceHost>();
            var settings = containerProvider.Resolve<ISettingsService>();
            var shotcuts = containerProvider.Resolve<IShortcutOverrideStore>();
            var builder = containerProvider.Resolve<IShortcutMapProvider>();
            var loggerCreator = containerProvider.Resolve<LoggerCreator>();
           
            initializer.Initialize();

            var token = new CancellationToken();

            backgroundService.Start(token);

            builder.Rebuild();

            _logger.Info("AppLoadModule initialized");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        private static void RegisterConsoleCommands(IContainerProvider containerProvider)
        {
            ValidateConsoleCommands(containerProvider);

            var commandProvider = containerProvider.Resolve<IConsoleCommandProvider>();
            var dispatcher = containerProvider.Resolve<ConsoleCommandDispatcher>();

            // Регистрируем все команды из сервиса
            foreach (var cmd in commandProvider.GetAllCommands())
            {
                dispatcher.RegisterCommand(cmd);
            }
        }

        private static void ValidateConsoleCommands(
            IContainerProvider containerProvider)
        {
            var commands =
                ConsoleCommandDiscovery.Discover(
                    typeof(EchoCommand).Assembly);

            foreach (var type in commands)
            {
                try
                {
                    containerProvider.Resolve(type);
                }
                catch (Exception ex)
                {
                    _logger.Info($"Console command failed: {type.FullName}\n{ex}");
                }
            }
        }

        private static void RegisterDiaglog(IContainerProvider containerRegistry)
        {
            var dialog = containerRegistry.Resolve<IDialogRegistry>();

            dialog.Register(new DialogDefinition(
                "core.show-settings",
                typeof(SettingsPanelView),
                typeof(SettingsPanelViewModel),
                new DialogOptions()
                {
                    Height = 800,
                    Width = 1000,
                    IsResizable = false,
                    Title = "Открыть диалог настроек"
                }
            ));

            dialog.Register(new DialogDefinition(
                "core.copy-dialog",
                typeof(CopyDialogView),
                typeof(CopyDialogViewModel),
                new DialogOptions()
                {
                    Height = 300,
                    Width = 500,
                    IsResizable = false,
                    Title = "Настройки копирования файлов"
                }
                ));

            dialog.Register(new DialogDefinition(
                 "core.copy-progress-dialog",
                 typeof(CopyProcessView),
                 typeof(CopyProcessViewModel),
                 new DialogOptions()
                 {
                     Height = 300,
                     Width = 500,
                     IsResizable = false,
                     Title = "Копирование файлов"
                 }
                 ));


            dialog.Register(new DialogDefinition(
                 "core.creation-item-dialog",
                 typeof(CreationDialogView),
                 typeof(CreationDialogViewModel),
                 new DialogOptions()
                 {
                     Height = 250,
                     Width = 300,
                     IsResizable = false,
                     Title = "Создание элемента"
                 }
                 ));
        }

        private static void RegisterCommand(IContainerProvider containerRegistry)
        {
            var registry = containerRegistry.Resolve<CommandRegistryService>();
            var windowManager = containerRegistry.Resolve<IWindowManager>();

            registry.Register(
                CommandFactoryExtensions.Create(
                    CommandNames.UI.ShowSettings,
                    async _ =>
                    {
                        windowManager.ShowDialog<SettingsPanelView>();
                        await Task.CompletedTask;
                    }));
        }

        private static void RegisterViewModels(IContainerProvider containerRegistry)
        {
            var viewRegistry = containerRegistry.Resolve<IViewRegistry>();

            viewRegistry.Register<CreateFolderViewModel, CreateFolderView>();
            viewRegistry.Register<CreateFileViewModel, CreateFileView>();
        }

        private static void RegisterShortcuts(IContainerProvider containerRegistry)
        {
            var shortcutBinder = containerRegistry.Resolve<IShortcutBinder>();

            shortcutBinder.Bind(CommandNames.File.Open, ShortcutKey.Enter);
            shortcutBinder.Bind(CommandNames.UI.ShowSettings, ShortcutKey.F12);
            shortcutBinder.Bind(CommandNames.UI.ToggleBottomPanel, ShortcutKey.Oem3, ShortcutModifiers.Ctrl);
            shortcutBinder.Bind(CommandNames.UI.ToggleRibbon, ShortcutKey.T, ShortcutModifiers.Ctrl);
            shortcutBinder.Bind(CommandNames.UI.ToggleSidebar, ShortcutKey.B, ShortcutModifiers.Ctrl);
            shortcutBinder.Bind(CommandNames.Navigation.GoUp, ShortcutKey.Backspace);
            shortcutBinder.Bind(CommandNames.File.Delete, ShortcutKey.Delete);
            shortcutBinder.Bind(CommandNames.File.Rename, ShortcutKey.F2);
            //shortcutBinder.Bind(CommandNames.File.Copy, ShortcutKey.C, ShortcutModifiers.Ctrl);
            //shortcutBinder.Bind(CommandNames.File.Paste, ShortcutKey.V, ShortcutModifiers.Ctrl);
            shortcutBinder.Bind(CommandNames.Panel.SelectAll, ShortcutKey.A, ShortcutModifiers.Ctrl);
            shortcutBinder.Bind(CommandNames.History.Redo, ShortcutKey.Y, ShortcutModifiers.Ctrl);
            shortcutBinder.Bind(CommandNames.History.Undo, ShortcutKey.Z, ShortcutModifiers.Ctrl);
        }
    }
}