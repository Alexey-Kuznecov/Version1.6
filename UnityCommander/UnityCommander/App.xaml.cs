
using Prism.Ioc;
using Prism.Modularity;
using System.Linq;
using System.Windows;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Bootstrap;
using UnityCommander.Common.Styling;
using UnityCommander.Dependencies;
using UnityCommander.Modules.BottomPanel;
using UnityCommander.Modules.FilePanel;
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
        private WindowInputManager _windowInput;

        protected override Window CreateShell()
        {
            var catalog = new ThemeCatalog();

            ThemeManager.Initialize(catalog, "Dark");

            var resources =
               CoreResources.ResourceUris
                   .Concat(ThemeManager.GetResourceUris())
                       .Concat(ModuleResources.ResourceUris);

            foreach (var dictionary in SharedDictionaryManager.Load(resources))
            {
                Resources.MergedDictionaries.Add(dictionary);
            }

            return this.Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell(Window shell)
        {
            _windowInput = this.Container.Resolve<WindowInputManager>();

            _windowInput.Attach(shell, ShortcutScope.MainWindow);

            base.InitializeShell(shell);
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
            SearchRegistration.Register(containerRegistry);
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
    }
}
