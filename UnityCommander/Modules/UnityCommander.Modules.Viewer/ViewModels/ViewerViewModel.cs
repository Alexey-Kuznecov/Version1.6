
namespace UnityCommander.Modules.Viewer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Prism.Mvvm;

    using UnityCommander.Common.Module;
    using UnityCommander.Core.Commands;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The viewer view model.
    /// </summary>
    public class ViewerViewModel : BindableBase, ITabPanelContent, IViewerPanel
    {
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the current file path.
        /// </summary>
        public string CurrentPath { get; set; }


        public object viewerContent;

        public event Action<string> PathChanged;

        /// <summary>
        /// Gets or sets the current directory.
        /// </summary>
        public object ViewerContent
        {
            get => this.viewerContent;
            set => this.SetProperty(ref this.viewerContent, value);
        }

        /// <summary>
        /// Устанавливает содержимое панели в ViewerView.
        /// </summary>
        /// <param name="content"></param>
        public void SetViewerContent(object content)
        {
            if (content != null)
            {
                var pluginSettingsView = new Views.PluginSettingsView();
                var context = pluginSettingsView.DataContext as IPluginSettingsViewer;
                context.SetPluginSettingsContent(content);
                this.ViewerContent = pluginSettingsView;
            }
        }

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
        public ITabPanelContent InitializedViewModel(ref Guid token, string path)
        {
            this.CurrentPath = path;
            this.Token = token;
            //this.navigationCommand = (NavigationInvoker)this.commandManager.CommandRegister(token, new NavigationInvoker());
            return this;
        }

        /// <summary>
        /// The get current path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCurrentPath()
        {
            return this.CurrentPath;
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

        /// <summary>
        /// The get current file path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCurrentFilePath()
        {
            return this.CurrentPath;
        }

        public void SetCurrentPath(string value)
        {
            throw new NotImplementedException();
        }
    }
}
