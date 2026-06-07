
using Prism.Ioc;
using Prism.Modularity;
using System.Linq;
using System.Windows;
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

namespace UnityCommander
{
    public partial class App
    {
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
            //AiRegistration.Register(containerRegistry);
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }
   
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Модули 
            moduleCatalog.AddModule<FilePanelModule>();       
            moduleCatalog.AddModule<LeftSideBarsModule>();
            moduleCatalog.AddModule<ToolBarModule>();
            moduleCatalog.AddModule<ViewerModule>();
            moduleCatalog.AddModule<SettingsPanelModule>();
            moduleCatalog.AddModule<WebBrowserModule>();
            moduleCatalog.AddModule<BottomPanelModule>();

            // Регистрация команд модулей
            moduleCatalog.AddModule<FilePanelCommandModule>(); // Команды

            // Инициализация после загрузки все модулей
            moduleCatalog.AddModule<AppLoadModule>();
        }
    }
}
