
using Prism.Ioc;
using Prism.Modularity;
using System.Threading;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Common.Dialog;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Services.Background;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Interfaces;
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
        public void OnInitialized(IContainerProvider containerProvider)
        {
            RegisterDiagnostics(containerProvider);
            RegisterBinding(containerProvider);
            RegisterDiaglog(containerProvider);
            var initializer = containerProvider.Resolve<AppInitializer>();
            var refreshService = containerProvider.Resolve<IBackgroundService>();
            var token = new CancellationToken();
            _ = refreshService.RunAsync(token);
            initializer.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        private static void RegisterDiagnostics(IContainerProvider containerRegistry)
        {
            var diagnostics = containerRegistry.Resolve<IDiagnosticRegistry>();

            var pan = containerRegistry.Resolve<IPanelRegistry>();
            var tab = containerRegistry.Resolve<ITabRegistry>();

            diagnostics.Register(pan as IDiagnosticSource);
            diagnostics.Register(tab as IDiagnosticSource);
        }

        private static void RegisterDiaglog(IContainerProvider containerRegistry)
        {
            var dialog = containerRegistry.Resolve<IDialogRegistry>();

            dialog.Register(new DialogDefinition(
                "core.copy-dialog", 
                typeof(CopyDialogView),
                typeof(CopyDialogViewModel), 
                new DialogOptions() 
                    { 
                        Height= 300, 
                        Width = 500, 
                        IsResizable=false, 
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

        private static void RegisterBinding(IContainerProvider containerRegistry)
        {
            var shortcut = containerRegistry.Resolve<IShortcutRegistry>();

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.UI.ToggleBottomPanel,
                Key = ShortcutKey.Oem3,
                Modifiers = ShortcutModifiers.Ctrl,
                Scope = ShortcutScope.Global
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.UI.ToggleRibbon,
                Key = ShortcutKey.T,
                Modifiers = ShortcutModifiers.Ctrl,
                Scope = ShortcutScope.Global
            });

            shortcut.Register(new ShortcutDefinition()
            {
                CommandId = CommandNames.UI.ToggleSidebar,
                Key = ShortcutKey.B,
                Modifiers = ShortcutModifiers.Ctrl,
                Scope = ShortcutScope.Global
            });
        }
    }
}