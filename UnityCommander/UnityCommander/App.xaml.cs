
using System;
using System.Windows.Navigation;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using UnityCommander.Modules.TabPanel.ViewModels;
using UnityCommander.ViewModels.Dialogs;

namespace UnityCommander
{
    using System.Windows;

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using Core;
    using Core.Commands;
    using Modules.LeftSideBars;
    using Modules.TabPanel;
    using Modules.TabPanel.Views;
    using Modules.ToolBar;
    using Services;
    using Services.Interfaces;
    using Services.Plugins;
    using ViewModels;
    using Views;
    using Views.CopyDialogs;

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
            containerRegistry.RegisterSingleton<IDialogService, OverrideDialogService>();
            containerRegistry.RegisterSingleton<IDataProviderService, DataProviderService>();
            containerRegistry.RegisterSingleton<IMultiCommandService, MultiCommandService>();
            containerRegistry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();
            containerRegistry.RegisterSingleton<IIconProviderService, IconProviderService>();
            containerRegistry.RegisterSingleton<IAppConfigService, AppConfigService>();

            // Commander Manager
            containerRegistry.RegisterSingleton<CommandManager>();
            containerRegistry.RegisterSingleton<ModuleLogger>();
        }

        protected override Rules CreateContainerRules()
        {
            ContainerLocator.Container.Resolve(typeof(PluginLoaderService));

            return base.CreateContainerRules();
        }

        //private IContainerExtension CreateContainerExtension() => new DryIocContainerExtension();

        //public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        //{
        //    ContainerLocator.SetContainerExtension(CreateContainerExtension);
        //    var container = ContainerLocator.Container;
        //    container.RegisterServices(services);
        //    RegisterTypes(container);
        //    return container.GetContainer();
        //}


        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IPluginLoaderService, PluginLoaderService>();
            containerRegistry.RegisterSingleton<IGlobalCommandService, GlobalCommandService>();

            base.RegisterRequiredTypes(containerRegistry);
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
