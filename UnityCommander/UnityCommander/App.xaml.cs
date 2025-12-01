
using DryIoc;
using UnityCommander.Modules.SettingsPanel;
using UnityCommander.Modules.TabPanel.ViewModels;
using UnityCommander.Modules.WebBrowser;
using UnityCommander.ViewModels.Dialogs;

namespace UnityCommander
{
    using CommandSystem.Core.Abstractions;
    using CommandSystem.Core.Factory;
    using CommandSystem.Gui.Integraion;
    using CommandSystem.Infrastructure.Execution;
    using CommandSystem.Infrastructure.Lifecycle;
    using Core;
    using Modules.LeftSideBars;
    using Modules.TabPanel.Views;
    using Modules.ToolBar;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using Services;
    using Services.Interfaces;
    using Services.Plugins;
    using System.Windows;
    using UnityCommander.AI.ImageSearch;
    using UnityCommander.Core.Navgator;
    using UnityCommander.Integration;
    using UnityCommander.Modules.BottomPanel;
    using UnityCommander.Modules.FilePanel;
    using UnityCommander.Modules.Viewer;
    using UnityCommander.Modules.Viewer.Views;
    using UnityCommander.Operation;
    using ViewModels;
    using Views;
    using Views.CopyDialogs;
    using ICommandExecutor = CommandSystem.Core.Abstractions.ICommandExecutor;
    using ICommandFactory = CommandSystem.Core.Abstractions.ICommandFactory;

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

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var pluginBridge = containerProvider.Resolve<PluginBridge>();
            pluginBridge.ExecuteCreate();
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
            // AI Services
            //containerRegistry.RegisterSingleton<IImageSimilarityService>(() =>
            //    new ImageSimilarityService(@"F:\01. Active\CSharp\UnityCommander\UnityCommander\Resources\ai_models"));


            containerRegistry.RegisterDialog<DialogView, DialogViewModel>("DialogPlugin");
            containerRegistry.RegisterDialog<CopyDialogView, CopyDialogViewModel>("CopyDialog");
            containerRegistry.RegisterDialog<CopyDialogSkipReplace, CopyDialogSkipReplaceViewModel>("CopyDialogSkipReplace");
            //containerRegistry.RegisterDialog<DialogPluginConfigView, DialogPluginConfigVm>("DialogPluginConfig");
            containerRegistry.RegisterDialog<AppConfigDialogControl, AppConfigDialogViewModel>("AppConfigDialog");
            containerRegistry.RegisterInstance(typeof(IPluginLoaderService), pluginLoaderService);
            containerRegistry.RegisterInstance(typeof(IGlobalCommandService), globalCommandService);
            containerRegistry.RegisterSingleton<IDockingService, DockingService>();
            containerRegistry.RegisterSingleton<IDialogService, OverrideDialogService>();
            containerRegistry.RegisterSingleton<IDataProviderService, DataProviderService>();
            containerRegistry.RegisterSingleton<IMultiCommandService, MultiCommandService>();
            containerRegistry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();
            containerRegistry.RegisterSingleton<IIconProviderService, IconProviderService>();
            containerRegistry.RegisterSingleton<IConsoleCommandProvider, ConsoleCommandProvider>();
            containerRegistry.RegisterSingleton<IDirectoryChangeNotifier, DirectoryChangeNotifier>();
            containerRegistry.RegisterSingleton<IAppConfigService, AppConfigService>();
            containerRegistry.RegisterSingleton<IAppLogger, AppLogger>();
            containerRegistry.RegisterSingleton<NavigationContextDirectory>();
            containerRegistry.RegisterSingleton<CopyProgressCalculator>();
            containerRegistry.RegisterSingleton<CopyReportCollector>();
            containerRegistry.RegisterSingleton<CopyConflictResolver>();
            containerRegistry.RegisterSingleton<CopyOperationController>();

            //containerRegistry.RegisterSingleton<IFileOperations, FileManager>();
            containerRegistry.Register<PluginBridge>();

            // Commander Manager
            //containerRegistry.RegisterSingleton<CommandManager>();
            containerRegistry.RegisterSingleton<ModuleLogger>();
           

            containerRegistry.RegisterSingleton<ICommandRegistry, CommandRegistry>();
            containerRegistry.RegisterSingleton<ICommandFactory, CommandFactory>();
            containerRegistry.RegisterSingleton<ICommandExecutor, CommandExecutor>();
            containerRegistry.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();

            containerRegistry.RegisterSingleton<GuiCommandRegistrar>();
            containerRegistry.RegisterSingleton<GuiCommandExecute>();
        }

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
        }

        /// <summary>
        /// The configure module catalog.
        /// </summary>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<FilePanelModule>();
            moduleCatalog.AddModule<LeftSideBarsModule>();
            moduleCatalog.AddModule<ToolBarModule>();
            moduleCatalog.AddModule<ViewerModule>();
            moduleCatalog.AddModule<SettingsPanelModule>();
            moduleCatalog.AddModule<WebBrowserModule>();
            moduleCatalog.AddModule<BottomPanelModule>();
        }
    }
}
