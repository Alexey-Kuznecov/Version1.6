
namespace UnityCommander
{
    using System.Windows;

    using Prism.DryIoc;
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
            containerRegistry.RegisterSingleton<IDialogService, OverrideDialogService>();
            containerRegistry.RegisterDialog<DialogView, DialogViewModel>();
            containerRegistry.Register<IDirectoryProviderService, DirectoryProviderService>();
            containerRegistry.Register<IGlobalCommandService, GlobalCommandService>();
            containerRegistry.Register<ISettingsProviderService, SettingsProviderService>();
            containerRegistry.Register<IIconProviderService, IconProviderService>();
            containerRegistry.Register<IPluginLoaderService, PluginLoaderService>();
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
