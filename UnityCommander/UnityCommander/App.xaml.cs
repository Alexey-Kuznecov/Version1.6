
using UnityCommander.Modules.TabPanel.ViewModels;
using UnityCommander.ViewModels.Dialogs;

namespace UnityCommander
{
    using System.Windows;

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using UnityCommander.Core;
    using UnityCommander.Core.Commands;
    using UnityCommander.Modules.LeftSideBars;
    using UnityCommander.Modules.TabPanel;
    using UnityCommander.Modules.TabPanel.Views;
    using UnityCommander.Modules.ToolBar;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Plugins;
    using UnityCommander.ViewModels;
    using UnityCommander.Views;
    using UnityCommander.Views.CopyDialogs;

    /// <summary>
    /// The application.
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// The create shell.
        /// </summary>
        /// <returns>
        /// The <see cref="Window"/>.
        /// </returns>
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
        
        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry">
        /// The container registry.
        /// </param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DialogView, DialogViewModel>("DialogPlugin");
            containerRegistry.RegisterDialog<CopyDialogView, CopyDialogViewModel>("CopyDialog");
            containerRegistry.RegisterDialog<DialogPluginConfigView, DialogPluginConfigVm>("DialogPluginConfig");
            containerRegistry.RegisterSingleton<IPluginLoaderService, PluginLoaderService>();
            containerRegistry.RegisterSingleton<IDialogService, OverrideDialogService>();
            containerRegistry.RegisterSingleton<IDataProviderService, DataProviderService>();
            containerRegistry.RegisterSingleton<IMultiCommandService, MultiCommandService>();
            containerRegistry.RegisterSingleton<IGlobalCommandService, GlobalCommandService>();
            containerRegistry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();
            containerRegistry.RegisterSingleton<IIconProviderService, IconProviderService>();
            containerRegistry.RegisterSingleton<IAppConfigService, AppConfigService>();

            // Commander Manager
            containerRegistry.RegisterSingleton<CommandManager>();
            containerRegistry.RegisterSingleton<ModuleLogger>();

            // containerRegistry.RegisterForNavigation<ViewA>("ViewA");
        }

        /// <summary>
        /// The configure view model locator.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            ViewModelLocationProvider.Register(typeof(LeftPanelContentView).ToString(), () => Container.Resolve<TabPanelViewModel>());
            ViewModelLocationProvider.Register(typeof(RightPanelContentView).ToString(), () => Container.Resolve<TabPanelViewModel>());
        }

        /// <summary>
        /// The configure module catalog.
        /// </summary>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<LeftSideBarsModule>();
            moduleCatalog.AddModule<ToolBarModule>();
            moduleCatalog.AddModule<TabPanelModule>();
        }
    }
}
