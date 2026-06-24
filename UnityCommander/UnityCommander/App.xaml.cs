
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Bootstrap;
using UnityCommander.Common.Styling;
using UnityCommander.Core.Theming;
using UnityCommander.Dependencies;
using UnityCommander.Modules.BottomPanel;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.SettingsPanel;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Modules.Viewer;
using UnityCommander.Modules.WebBrowser;
using UnityCommander.Views;
using UnityCommander.WPF.Dialog;

namespace UnityCommander
{
    public partial class App
    {
        private WindowInputManager _windowInput;

        private Window? _activeWindow;

        protected override Window CreateShell()
        {
            var catalog = new ThemeCatalog();

            ThemeManager.Initialize(catalog, "Material");

            //var resources =
            //    ThemeManager.GetResourceUris()
            //        .Concat(ModuleResources.ResourceUris);

            foreach (var dictionary in SharedDictionaryManager.Load(ModuleResources.ResourceUris))
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

            // Регистрация команд модулей
            moduleCatalog.AddModule<FilePanelCommandModule>(); // Команды

            // Инициализация после загрузки все модулей
            moduleCatalog.AddModule<AppLoadModule>();
        }
    }
}
