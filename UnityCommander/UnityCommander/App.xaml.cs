
using System;
using System.Collections.Generic;
using System.Windows.Navigation;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using UnityCommander.Modules.SettingsPanel;
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
    using UnityCommander.Modules.Viewer;
    using UnityCommander.Modules.Viewer.ViewModels;
    using UnityCommander.Modules.Viewer.Views;



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
            var pluginLoaderService = new PluginLoaderService();
            var globalCommandService = new GlobalCommandService(pluginLoaderService);

            containerRegistry.RegisterDialog<DialogView, DialogViewModel>("DialogPlugin");
            containerRegistry.RegisterDialog<CopyDialogView, CopyDialogViewModel>("CopyDialog");
            containerRegistry.RegisterDialog<CopyDialogSkipReplace, CopyDialogSkipReplaceViewModel>("CopyDialogSkipReplace");
            containerRegistry.RegisterDialog<DialogPluginConfigView, DialogPluginConfigVm>("DialogPluginConfig");
            containerRegistry.RegisterInstance(typeof(IPluginLoaderService), pluginLoaderService);
            containerRegistry.RegisterInstance(typeof(IGlobalCommandService), globalCommandService);
            containerRegistry.RegisterSingleton<IDialogService, OverrideDialogService>();
            containerRegistry.RegisterSingleton<IDataProviderService, DataProviderService>();
            containerRegistry.RegisterSingleton<IMultiCommandService, MultiCommandService>();
            containerRegistry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();
            containerRegistry.RegisterSingleton<IIconProviderService, IconProviderService>();
            containerRegistry.RegisterSingleton<IAppConfigService, AppConfigService>();
            
            // Commander Manager
            containerRegistry.RegisterSingleton<CommandManager>();
            containerRegistry.RegisterSingleton<ModuleLogger>();

            //Container.UseInstance(typeof(IPluginLoaderService),
            //    new PluginLoaderService(),
            //    IfAlreadyRegistered.Replace, false, true, 1);
        }

        //protected override Rules CreateContainerRules()
        //{
        //    return Rules.Default
        //        .WithAutoConcreteTypeResolution(Condition)
        //        .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
        //        .WithoutThrowOnRegisteringDisposableTransient()
        //        .WithFuncAndLazyWithoutRegistration()
        //        .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);
        //}

        private bool Condition(Request arg)
        {
            return true;
        }

        #region Research

        //private bool Condition(Request arg)
        //{
        //    arg.Container.Use(typeof(IPluginLoaderService), Instance);
        //    return true;
        //}

        //private object Instance(IResolverContext r)
        //{
        //    return null;
        //}

        //private Factory Rule(Request request, KeyValuePair<object, Factory>[] factories)
        //{
        //    var f = default(Factory);

        //    foreach (var factory in factories)
        //    {
        //        var ddd = factory.Value.GetDelegateOrDefault(request);
        //        var dd2d = factory.Value.GetExpressionOrDefault(request);
        //        request.WithResolvedFactory(factory.Value);
        //        var dd = Factory.GetNextID();
        //    }

        //    return null;
        //}

        #endregion

        /// <summary>
        /// The configure view model locator.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            ViewModelLocationProvider.Register(typeof(LeftPanelContentView).ToString(), () => Container.Resolve<TabPanelViewModel>());
            //ViewModelLocationProvider.Register(typeof(RightPanelContentView).ToString(), () => Container.Resolve<TabPanelViewModel>());
            //ViewModelLocationProvider.Register(typeof(ViewerView).ToString(), () => Container.Resolve<ViewerViewModel>());
        }

        /// <summary>
        /// The configure module catalog.
        /// </summary>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<TabPanelModule>();
            moduleCatalog.AddModule<LeftSideBarsModule>();
            moduleCatalog.AddModule<ToolBarModule>();
            moduleCatalog.AddModule<ViewerModule>();
            moduleCatalog.AddModule<SettingsPanelModule>();
        }
    }
}
