
using UnityCommander.Services.Plugins;

namespace UnityCommander
{
    using System.Windows;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Services.Dialogs;

    using UnityCommander.Modules.FilePanel;
    using UnityCommander.Modules.LeftSideBars;
    using UnityCommander.Modules.ToolBar;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.ViewModels;
    using UnityCommander.Views;


#if NETCOREAPP3_1
    using UnityCommander.Services.Plugins.NETCORE3_1;
#elif NET472
    using UnityCommander.Services.Plugins.NET48;
#endif
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
            return this.Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry">
        /// The container registry.
        /// </param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DialogView, DialogViewModel>();
            containerRegistry.RegisterSingleton<IDialogService, OverrideDialogService>();
            containerRegistry.RegisterSingleton<IDirectoryProviderService, DirectoryProviderService>();
            containerRegistry.RegisterSingleton<IGlobalCommandService, GlobalCommandService>();
            containerRegistry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();
            containerRegistry.RegisterSingleton<IIconProviderService, IconProviderService>();
            containerRegistry.RegisterSingleton<IPluginLoaderService, PluginLoaderService>();
            //containerRegistry.RegisterSingleton<IPluginManagerService, PluginManagerService>();
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
        }
    }
}
