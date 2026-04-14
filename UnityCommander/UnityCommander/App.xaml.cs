using CommandSystem.Abstractions;
using CommandSystem.Core.Factory;
using CommandSystem.Core.UndoRedo;
using CommandSystem.Gui.Integraion;
using CommandSystem.Infrastructure.Execution;
using CommandSystem.Infrastructure.Lifecycle;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Windows;
using UnityCommander.Abstractions.Completion;
using UnityCommander.AI.ImageSearch;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Autocomplete.Infrastructure;
using UnityCommander.Autocomplete.Infrastructure.Analyze;
using UnityCommander.Commands;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Selection;
using UnityCommander.Core;
using UnityCommander.Core.Behaviors.Selection;
using UnityCommander.Core.Navgator;
using UnityCommander.Integration.Plugins;
using UnityCommander.Logging;
using UnityCommander.Logging.Abstractions;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Filters;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Logging.Sinks;
using UnityCommander.Modules.BottomPanel;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.SettingsPanel;
using UnityCommander.Modules.TabPanel.ViewModels;
using UnityCommander.Modules.TabPanel.Views;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Modules.Viewer;
using UnityCommander.Modules.Viewer.Views;
using UnityCommander.Modules.WebBrowser;
using UnityCommander.Operation;
using UnityCommander.Ribbon.Core.Services;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Settings;
using UnityCommander.Services.Selection;
using UnityCommander.Services.Settings;
using UnityCommander.Sinks;
using UnityCommander.ViewModels;
using UnityCommander.ViewModels.Dialogs;
using UnityCommander.Views;
using UnityCommander.Views.CopyDialogs;

namespace UnityCommander
{
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
            containerRegistry.RegisterSingleton<IConsoleCommandProvider, ConsoleCommandProvider>();

            containerRegistry.RegisterSingleton<LogHub>();

            var settings = new GlobalLoggerSettings
            {
                Mode = LoggingMode.Debug,
                MinimumLevel = LogLevel.Debug,
                EnabledScopes = new HashSet<string>
                {
                    "Startup",
                    "Runtime"
                },
                EnabledCategories = new HashSet<string>
                {
                    "System",
                    "Plugin"
                }
            };

            containerRegistry.RegisterInstance(settings);
            containerRegistry.RegisterSingleton<ILogSink, NullSink>();
            containerRegistry.RegisterSingleton<ILogSink>(_ => new FileLogSink("journal.log", LogChannel.Journal));
            containerRegistry.RegisterSingleton<ILogSink>(_ => new FileLogSink("errors.log", LogChannel.Error));
            containerRegistry.RegisterSingleton<ILogFilter, LoggingPolicyFilter>();
            containerRegistry.RegisterSingleton<ILogColorResolver, DefaultLogColorResolver>();
            containerRegistry.RegisterSingleton<LoggerCreator>();
            containerRegistry.RegisterSingleton<LoggingSinkService>();


            var pluginCommand = new SimpleCommandDescriptor(
               name: "plugin",
               variants: new[]
               {
                // ─── Load ─────────────────────
                new CommandVariant(
                    name: "load",
                    flags: new[]
                    {
                        new SimpleFlagDescriptor(
                            name: "--force",
                            shortName: "-f",
                            requiresValue: true,
                            valueType: ArgumentValueType.Boolean),
                        new SimpleFlagDescriptor(
                            name: "--dependencies",
                            shortName: "-d",
                            requiresValue: false,
                            valueType: ArgumentValueType.Boolean)
                    },
                    arguments: new List<IPositionalArgumentDescriptor>
                    {
                        new SimplePositionalArgumentDescriptor(
                            name: "path",
                            valueType: ArgumentValueType.Path,
                            isRequired: true)
                    },
                    flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                    usage: "plugin load <path> [--force] [--dependencies]"
                ),

                // ─── Unload ─────────────────────
                new CommandVariant(
                    name: "unload",
                    flags: new[]
                    {
                        new SimpleFlagDescriptor(
                            name: "--all",
                            shortName: "-a",
                            requiresValue: false,
                            valueType: ArgumentValueType.Boolean),
                        new SimpleFlagDescriptor(
                            name: "--force",
                            shortName: "-f",
                            requiresValue: false,
                            valueType: ArgumentValueType.Boolean)
                    },
                    arguments: new List<IPositionalArgumentDescriptor>
                    {
                        new SimplePositionalArgumentDescriptor(
                            name: "name",
                            valueType: ArgumentValueType.String,
                            isRequired: false)
                    },
                    flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                    usage: "plugin unload [name] [--all] [--force]"
                ),

                // ─── Reload ─────────────────────
                new CommandVariant(
                    name: "reload",
                    flags: new[]
                    {
                        new SimpleFlagDescriptor(
                            name: "--all",
                            shortName: "-a",
                            requiresValue: false,
                            valueType: ArgumentValueType.Boolean)
                    },
                    arguments: new List<IPositionalArgumentDescriptor>
                    {
                        new SimplePositionalArgumentDescriptor(
                            name: "name",
                            valueType: ArgumentValueType.String,
                            isRequired: false)
                    },
                    flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                    usage: "plugin reload [name] [--all]"
                ),

                // ─── List ─────────────────────
                new CommandVariant(
                    name: "list",
                    flags: new[]
                    {
                        new SimpleFlagDescriptor(
                            name: "--verbose",
                            shortName: "-v",
                            requiresValue: false,
                            valueType: ArgumentValueType.Boolean)
                    },
                    arguments: new List<IPositionalArgumentDescriptor>(), // путь не нужен для list
                    flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                    usage: "plugin list [--verbose]"
                ),

                // ─── Info ─────────────────────
                new CommandVariant(
                    name: "info",
                    flags: new[]
                    {
                        new SimpleFlagDescriptor(
                            name: "--all",
                            shortName: "-a",
                            requiresValue: false,
                            valueType: ArgumentValueType.Boolean)
                    },
                    arguments: new List<IPositionalArgumentDescriptor>
                    {
                        new SimplePositionalArgumentDescriptor(
                            name: "name",
                            valueType: ArgumentValueType.String,
                            isRequired: true)
                    },
                    flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                    usage: "plugin info <name> [--all]"
                )
               }
           );

            var gitCommand = new SimpleCommandDescriptor(
                name: "git",
                variants: new[]
                {
                    new CommandVariant(
                        name: "commit",
                        flags: new[]
                        {
                            new SimpleFlagDescriptor(
                                name: "-m",
                                shortName: null,
                                requiresValue: true,
                                valueType: ArgumentValueType.String),
                            new SimpleFlagDescriptor(
                                name: "--amend",
                                shortName: null,
                                requiresValue: false)
                        },
                        arguments : new List<IPositionalArgumentDescriptor>
                        {
                            new SimplePositionalArgumentDescriptor("message", ArgumentValueType.String)
                        },
                        flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                        usage: "git commit <message> [-m <message>] [--amend]"),
                    new CommandVariant(
                        name: "push",
                        flags: new[]
                        {
                            new SimpleFlagDescriptor(
                                name: "--all",
                                shortName: null,
                                requiresValue: false),

                            new SimpleFlagDescriptor(
                                name: "-a",
                                shortName: null,
                                requiresValue: false,
                                valueType: ArgumentValueType.String)
                        },
                        arguments : new List<IPositionalArgumentDescriptor>
                        {
                            new SimplePositionalArgumentDescriptor("remote", ArgumentValueType.String)
                        },
                        flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                        usage: "git push <message> [-m <message>] [--amend]"
                        )
                    }
                );

            //containerRegistry.RegisterInstance<ICommandDescriptor>(commitCommand);
            //containerRegistry.RegisterInstance<ICommandDescriptor>(gitCommitCommand);
            containerRegistry.RegisterInstance<ICommandDescriptor>(gitCommand);
            containerRegistry.RegisterInstance<ICommandDescriptor>(pluginCommand);

            containerRegistry.RegisterSingleton<Autocomplete.Infrastructure.Analyze.ICliInputAnalyzer, CliInputAnalyzer>();
            containerRegistry.RegisterSingleton<ICliParseStateBuilder, CliParseStateBuilder>();
            //containerRegistry.RegisterSingleton<ICliInputAnalyzer, DefaultCliInputAnalyzer>();

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
            containerRegistry.RegisterSingleton<IHistoryStore, InMemoryHistoryStore>();
            containerRegistry.RegisterSingleton<IHistoryManager, CommandHistoryManager>();

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
