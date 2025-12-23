
using UnityCommander.Modules.SettingsPanel;
using UnityCommander.Modules.TabPanel.ViewModels;
using UnityCommander.Modules.WebBrowser;
using UnityCommander.ViewModels.Dialogs;

namespace UnityCommander
{
    using CommandSystem.Abstractions;
    using CommandSystem.Core.Factory;
    using CommandSystem.Gui.Integraion;
    using CommandSystem.Infrastructure.Execution;
    using CommandSystem.Infrastructure.Lifecycle;
    using Core;
    using Example;
    using Modules.LeftSideBars;
    using Modules.TabPanel.Views;
    using Modules.ToolBar;
    using PluginSystem.Abstractions.Plugin;
    using PluginSystem.Abstractions.Services;
    using PluginSystem.Runtime;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using UnityCommander.AI.ImageSearch;
    using UnityCommander.Commands;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Plugins;
    using UnityCommander.Common.Selection;
    using UnityCommander.Core.Behaviors.Selection;
    using UnityCommander.Core.Navgator;
    using UnityCommander.Integration.Plugins;
    using UnityCommander.Logging;
    using UnityCommander.Modules.BottomPanel;
    using UnityCommander.Modules.FilePanel;
    using UnityCommander.Modules.FilePanel.Columns;
    using UnityCommander.Modules.Viewer;
    using UnityCommander.Modules.Viewer.Views;
    using UnityCommander.Operation;
    using UnityCommander.Ribbon.Core.Services;
    using UnityCommander.Services.Interfaces.Settings;
    using UnityCommander.Services.Selection;
    using UnityCommander.Services.Settings;
    using ViewModels;
    using Views;
    using Views.CopyDialogs;

    public partial class App
    {
        protected override Window CreateShell()
        {
            foreach (var dictionary in SharedDictionaryManager.SharedDictionary)
            {
                this.Resources.MergedDictionaries.Add(dictionary);
            }
            
            return this.Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // -------------------------------
            // 1. Создание сервисов вручную
            // -------------------------------
            // Сервис загрузки плагинов
            var pluginLoaderService = new PluginLoaderService();
            // Сервис глобальных команд, который зависит от загрузчика плагинов
            var globalCommandService = new GlobalCommandService(pluginLoaderService);

            // -------------------------------
            // 2. Регистрация диалогов
            // -------------------------------
            // Каждый диалог регистрируется с View и ViewModel
            containerRegistry.RegisterDialog<DialogView, DialogViewModel>("DialogPlugin");
            containerRegistry.RegisterDialog<CopyDialogView, CopyDialogViewModel>("CopyDialog");
            containerRegistry.RegisterDialog<CopyDialogSkipReplace, CopyDialogSkipReplaceViewModel>("CopyDialogSkipReplace");
            //containerRegistry.RegisterDialog<DialogPluginConfigView, DialogPluginConfigVm>("DialogPluginConfig"); // пока закомментирован
            containerRegistry.RegisterDialog<AppConfigDialogControl, AppConfigDialogViewModel>("AppConfigDialog");

            // -------------------------------
            // 3. Регистрация заранее созданных экземпляров
            // -------------------------------
            // Регистрация сервисов, которые уже созданы
            containerRegistry.RegisterInstance(typeof(IPluginLoaderService), pluginLoaderService);
            containerRegistry.RegisterInstance(typeof(IGlobalCommandService), globalCommandService);

            // -------------------------------
            // 4. Регистрация синглтонов (один экземпляр на приложение)
            // -------------------------------
            containerRegistry.RegisterSingleton<IDockingService, DockingService>();
            containerRegistry.RegisterSingleton<IDialogService, OverrideDialogService>();
            containerRegistry.RegisterSingleton<IDataProviderService, DataProviderService>();
            containerRegistry.RegisterSingleton<IMultiCommandService, MultiCommandService>();
            containerRegistry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();
            containerRegistry.RegisterSingleton<IIconProviderService, IconProviderService>();
            containerRegistry.RegisterSingleton<IDirectoryChangeNotifier, DirectoryChangeNotifier>();
            containerRegistry.RegisterSingleton<IAppConfigService, AppConfigService>();
            containerRegistry.RegisterSingleton<IPanelRegistry, PanelRegistry>();
            containerRegistry.RegisterSingleton<IAppLogger, AppLogger>();
            containerRegistry.RegisterSingleton<IConsoleCommandProvider, ConsoleCommandProvider>();

            //containerRegistry.RegisterSingleton<ILoggerService, NLogLoggerService>();
            containerRegistry.RegisterSingleton<IPluginManager, PluginManager>();
            containerRegistry.RegisterSingleton<IPluginProvider, PluginProvider>();

            // Команды
            containerRegistry.Register<IConsoleCommandBase, EchoCommand>();
            containerRegistry.Register<IConsoleCommandBase, SysStatCommand>();
            containerRegistry.Register<IConsoleCommandBase, TestCommand>();
            containerRegistry.Register<IConsoleCommandBase, ProcessControlCommand>();
            containerRegistry.Register<IConsoleCommandBase, SelectFilesCommand>();
            containerRegistry.Register<IConsoleCommandBase, FindSimilarCommand>();
            containerRegistry.Register<IConsoleCommandBase, FileUnlockCommand>();
            containerRegistry.Register<IConsoleCommandBase, TestFlashCommand>();
            containerRegistry.Register<IConsoleCommandBase, PluginConsoleCommand>();

            // Навигационный контекст, нужен один на всё приложение
            containerRegistry.RegisterSingleton<NavigationContextDirectory>();

            // Калькуляторы и контроллеры для копирования файлов
            containerRegistry.RegisterSingleton<CopyProgressCalculator>();
            containerRegistry.RegisterSingleton<CopyReportCollector>();
            containerRegistry.RegisterSingleton<CopyConflictResolver>();
            containerRegistry.RegisterSingleton<CopyOperationController>();

            // -------------------------------
            // 5. Регистрация обычных (не синглтонов) объектов
            // -------------------------------
            // Регистрируем каждую стратегию отдельно
            containerRegistry.RegisterSingleton<ISelectionStrategy, SingleClickSelectionStrategy>();
            containerRegistry.RegisterSingleton<ISelectionStrategy, ShiftSelectionStrategy>();
            containerRegistry.RegisterSingleton<ISelectionStrategy, CtrlSelectionStrategy>();
            containerRegistry.RegisterSingleton<ISelectionStrategy, ExtensionSelectionRuleStrategy>();

            containerRegistry.RegisterSingleton<IRibbonManager, RibbonManager>();

            // Службы для управления выделением в файловых панелях
            containerRegistry.RegisterSingleton<ISelectionService, SelectionService>();
            containerRegistry.Register<ISelectionManager, SelectionManager>();
            //containerRegistry.Register<PluginBridge>();

            // Колонки по умолчанию для файлового менеджера
            containerRegistry.RegisterSingleton<IColumnProvider, DefaultColumnProvider>();
            containerRegistry.Register<IColumnStateManager, ColumnStateManager>(); // по панели
            containerRegistry.Register<ISettingsStore, InMemorySettingsStore>(); // глобально
            containerRegistry.Register<ColumnRegistry>(); // зависит от задач

            // -------------------------------
            // 6. Логгеры и менеджеры
            // -------------------------------
            //containerRegistry.RegisterSingleton<CommandManager>(); // пока закомментирован
            containerRegistry.RegisterSingleton<ModuleLogger>();

            // -------------------------------
            // 7. Регистрация компонентов командной системы
            // -------------------------------
            containerRegistry.RegisterSingleton<ICommandRegistry, CommandRegistry>();
            containerRegistry.RegisterSingleton<ICommandFactory, CommandFactory>();
            containerRegistry.RegisterSingleton<ICommandExecutor, CommandExecutor>();
            containerRegistry.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();

            // -------------------------------
            // 8. Регистрация GUI-команд
            // -------------------------------
            containerRegistry.RegisterSingleton<ICommandRegister, GuiCommandRegister>();
            containerRegistry.RegisterSingleton<IGuiCommandExecutor, GuiCommandExecuter>();
            containerRegistry.RegisterSingleton<CommandService>();

            // -------------------------------
            // 9. AI сервисы (пока закомментированы)
            // -------------------------------
            containerRegistry.RegisterSingleton<IImageSimilarityService>(() =>
                new ImageSimilarityService(@"F:\\01. Active\\CSharp\\UnityCommander\\UnityCommander\\Resources\\ai_models\\model.onnx"));
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            ViewModelLocationProvider.Register(typeof(LeftPanelContentView).ToString(), () => Container.Resolve<TabPanelViewModel>());
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<FilePanelModule>();        // Панели
            moduleCatalog.AddModule<FilePanelCommandModule>(); // Команды
            moduleCatalog.AddModule<LeftSideBarsModule>();
            moduleCatalog.AddModule<ToolBarModule>();
            moduleCatalog.AddModule<ViewerModule>();
            moduleCatalog.AddModule<SettingsPanelModule>();
            moduleCatalog.AddModule<WebBrowserModule>();
            moduleCatalog.AddModule<BottomPanelModule>();
        }
    }
}
