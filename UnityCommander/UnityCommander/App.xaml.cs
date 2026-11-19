
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Bootstrap;
using UnityCommander.Common.Styling;
using UnityCommander.Dependencies;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Abstraction.Performance;
using UnityCommander.Diagnostics.Storage;
using UnityCommander.Logging.Contracts;
using UnityCommander.Modules.BottomPanel;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.FilePanel.Converters;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.SettingsPanel;
using UnityCommander.Modules.StatusBar;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Modules.Viewer;
using UnityCommander.Modules.WebBrowser;
using UnityCommander.Moduls;
using UnityCommander.Theme;
using UnityCommander.Views;
using UnityCommander.WPF.Dialog;

namespace UnityCommander
{
    public partial class App
    {
        private static ILogger _logger;

        private IPerformanceProfiler? _performanceProfiler;

        private IDiagnosticsRecorder? _diagnosticsRecorder;

        private WindowInputManager _windowInput;

        protected override Window CreateShell()
        {
            _logger = LoggingBootstrap.Initialize(Container);

#if UNITYCOMMANDER_DIAGNOSTICS
            StartDiagnosticsSession();
#endif
            var profiler = Container.Resolve<IPerformanceProfiler>();

            _performanceProfiler = profiler;

            using var startupScope =
                profiler.Measure("Startup.CreateShell");

            ThemeCatalog catalog;

            using (profiler.Measure("Startup.ThemeCatalog"))
            {
                catalog = new ThemeCatalog();
            }

            ThumbnailImageConverter converter;

            using (profiler.Measure("Startup.ThumbnailConverter"))
            {
                converter = Container.Resolve<ThumbnailImageConverter>();
            }

            Resources["ThumbnailImageConverter"] = converter;

            using (profiler.Measure("Startup.ThemeInitialization"))
            {
                ThemeManager.Initialize(catalog, "Dark");
            }

            using (profiler.Measure("Startup.ResourceDictionaries"))
            {
                var resources = CoreResources.ResourceUris
                    .Concat(ThemeManager.GetResourceUris())
                    .Concat(ModuleResources.ResourceUris)
                    .ToArray();

                foreach (var dictionary in SharedDictionaryManager.Load(resources))
                {
                    Resources.MergedDictionaries.Add(dictionary);
                }
            }

            MainWindow mainWindow;

            using (profiler.Measure("Startup.MainWindow"))
            {
                mainWindow = Container.Resolve<MainWindow>();
            }

            _logger.Info(
                $"Diagnostics dropped: {((DiagnosticsRecorder)_diagnosticsRecorder)?.DroppedRecordCount ?? 0}");

            _logger.Info(
                $"Diagnostics storage error: " +
                $"{((DiagnosticsRecorder)_diagnosticsRecorder)?.LastStorageError?.ToString() ?? "None"}");

            return mainWindow;
        }

        protected override void InitializeShell(Window shell)
        {
            var profiler = _performanceProfiler
                ?? Container.Resolve<IPerformanceProfiler>();

            using var startupScope =
                profiler.Measure("Startup.InitializeShell");

            using (profiler.Measure("Startup.WindowInputManager"))
            {
                _windowInput = Container.Resolve<WindowInputManager>();
            }

            using (profiler.Measure("Startup.AttachWindowInput"))
            {
                _windowInput.Attach(shell, ShortcutScope.MainWindow);
            }

            using (profiler.Measure("Startup.BaseInitializeShell"))
            {
                base.InitializeShell(shell);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            AppInfrastructureRegistration.Register(containerRegistry);
            CLIInfrastructureRegistration.Register(containerRegistry);
            CommandRegistration.Register(containerRegistry);
            ConsoleCommandRegistration.Register(containerRegistry);
            DialogModuleRegistration.Register(containerRegistry);
            FilePanelRegistration.Register(containerRegistry);
            LoggingModuleRegistration.Register(containerRegistry);
            PluginModuleRegistration.Register(containerRegistry);
            AutocompleteRegistration.Register(containerRegistry);
            CopyModuleRegistration.Register(containerRegistry);
            SettingsRegistration.Register(containerRegistry);
            BottomPanelRegistration.Register(containerRegistry);
            PreviewRegistration.Register(containerRegistry);
            SearchRegistration.Register(containerRegistry);
            NotificationRegistration.Register(containerRegistry);

            //AiRegistration.Register(containerRegistry);
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Инициализация до загрузки всех модулей
            moduleCatalog.AddModule<EarlyLoadModule>();

            // Модули 
            moduleCatalog.AddModule<FilePanelModule>();       
            moduleCatalog.AddModule<LeftSideBarsModule>();
            moduleCatalog.AddModule<BottomPanelModule>();
            // ВАЖНО: Ribbon инициализируется после нижней панели и после всех модулей, которые регистрируют свои команды в конструкторе.
            // Это необходимо для того, чтобы Ribbon успел построиться и корректно разрешить команды, предоставляемые модулями.
            moduleCatalog.AddModule<ToolBarModule>();
            moduleCatalog.AddModule<ViewerModule>();
            moduleCatalog.AddModule<SettingsPanelModule>();
            moduleCatalog.AddModule<WebBrowserModule>();
            moduleCatalog.AddModule<StatusBarModule>();

            // Регистрация команд модулей
            //moduleCatalog.AddModule<FilePanelCommandModule>();

            // Инициализация после загрузки все модулей
            moduleCatalog.AddModule<AppLoadModule>();
            moduleCatalog.AddModule<CommandRegistrationModule>(); // Команды
        }

        private void StartDiagnosticsSession()
        {
            try
            {
                _diagnosticsRecorder = Container.Resolve<IDiagnosticsRecorder>();

                var sessionInfo = new DiagnosticSessionInfo(
                    "Development",
                    typeof(App).Assembly.GetName().Version?.ToString()
                        ?? "Unknown",
                    null,
                    "Debug");

                Task.Run(async () =>
                {
                    await _diagnosticsRecorder
                        .StartSessionAsync(sessionInfo)
                        .ConfigureAwait(false);
                }).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                _logger.Error("Error while starting diagnostics session.", exception);

                _diagnosticsRecorder = null;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try

            {
                #if UNITYCOMMANDER_DIAGNOSTICS

                    if (_diagnosticsRecorder is not null)
                    {
                        Task.Run(async () =>
                        {
                            await _diagnosticsRecorder
                                .DisposeAsync()
                                .ConfigureAwait(false);
                        }).GetAwaiter().GetResult();
                    }
#endif
            }
            catch (Exception exception)
            {
                _logger.Error("Error while disposing diagnostics recorder.", exception);
            }
            finally
            {
                base.OnExit(e);
            }
        }
    }
}
