

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
using UnityCommander.Core.Diagnostics;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel;
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

            var initializer = containerProvider.Resolve<AppInitializer>();
            var backgroundService = containerProvider.Resolve<BackgroundServiceHost>();
            var settings = containerProvider.Resolve<ISettingsService>();
            var shotcuts = containerProvider.Resolve<IShortcutOverrideStore>();
            var builder = containerProvider.Resolve<IShortcutMapProvider>();
            var loggerCreator = containerProvider.Resolve<LoggerCreator>();

            initializer.Initialize();

            var token = new CancellationToken();

            backgroundService.Start(token);

            var selectionDiagnostics = containerProvider.Resolve<SelectionDiagnostics>();

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

        private static void RegisterShortcuts(IContainerProvider containerRegistry)
        {
            var shortcut = containerRegistry.Resolve<IShortcutRegistry>();

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.UI.ShowSettings,
                Description = CommandPresentationProvider.Get(CommandNames.UI.ShowSettings).Description,
                Key = ShortcutKey.F12,
                Modifiers = ShortcutModifiers.None,
                Scopes = ShortcutScope.FilePanel | ShortcutScope.MainWindow,
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.UI.ToggleBottomPanel,
                Description = CommandPresentationProvider.Get(CommandNames.UI.ToggleBottomPanel).Description,
                Key = ShortcutKey.Oem3,
                Modifiers = ShortcutModifiers.Ctrl,
                Scopes = ShortcutScope.Console | ShortcutScope.MainWindow,
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.UI.ToggleRibbon,
                Description = CommandPresentationProvider.Get(CommandNames.UI.ToggleRibbon).Description,
                Key = ShortcutKey.T,
                Modifiers = ShortcutModifiers.Ctrl,
                Scopes = ShortcutScope.MainWindow,
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.UI.ToggleSidebar,
                Description = CommandPresentationProvider.Get(CommandNames.UI.ToggleSidebar).Description,
                Key = ShortcutKey.B,
                Modifiers = ShortcutModifiers.Ctrl,
                Scopes = ShortcutScope.Sidebar | ShortcutScope.MainWindow,
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.History.Undo,
                Description = CommandPresentationProvider.Get(CommandNames.History.Undo).Description,
                Key = ShortcutKey.Z,
                Modifiers = ShortcutModifiers.Ctrl,
                Scopes = ShortcutScope.FilePanel | ShortcutScope.MainWindow,
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.History.Redo,
                Description = CommandPresentationProvider.Get(CommandNames.History.Redo).Description,
                Key = ShortcutKey.Y,
                Modifiers = ShortcutModifiers.Ctrl,
                Scopes = ShortcutScope.FilePanel | ShortcutScope.MainWindow,
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.File.Delete,
                Description = CommandPresentationProvider.Get(CommandNames.File.Delete).Description,
                Key = ShortcutKey.Delete,
                Modifiers = ShortcutModifiers.None,
                Scopes = ShortcutScope.FilePanel | ShortcutScope.MainWindow,
            });
        }
    }
}