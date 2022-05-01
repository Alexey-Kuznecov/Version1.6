
namespace UnityCommander.Modules.Viewer.ViewModels
{
    using System;

    using Prism.Mvvm;

    using UnityCommander.Common.Module;
    using UnityCommander.Core.Commands;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The viewer view model.
    /// </summary>
    public class ViewerViewModel : BindableBase, ITabPanelContent
    {
        /// <summary>
        /// The command manager.
        /// </summary>
        private readonly CommandManager commandManager;

        /// <summary>
        /// The navigation command.   
        /// </summary>
        private NavigationInvoker navigationCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewerViewModel"/> class.
        /// </summary>
        /// <param name="pluginLoaderService">
        /// The plugin loader service.
        /// </param>
        /// <param name="globalCommandService">
        /// The global command service.
        /// </param>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public ViewerViewModel(IPluginLoaderService pluginLoaderService, IGlobalCommandService globalCommandService, CommandManager manager)
        {
            this.commandManager = manager;
        }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// The initialized view model.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="ITabPanelContent"/>.
        /// </returns>
        public ITabPanelContent InitializedViewModel(Guid token, string path)
        {
            this.Token = token;
            this.navigationCommand = (NavigationInvoker)this.commandManager.CommandRegister(token, new NavigationInvoker());
            return this;
        }

        /// <summary>
        /// The get panel token.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetPanelToken()
        {
            return this.Token;
        }
    }
}
