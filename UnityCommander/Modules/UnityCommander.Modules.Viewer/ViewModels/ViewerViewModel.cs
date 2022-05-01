using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Core.Commands;
using UnityCommander.Core.Modules;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.Viewer.ViewModels
{
    public class ViewerViewModel : BindableBase, ITabPanelContent
    {

        private string path;

        /// <summary>
        /// The navigation command.   
        /// </summary>
        private NavigationInvoker navigationCommand;
        
        private CommandManager commandManager;

        public ViewerViewModel(IPluginLoaderService pluginLoaderService, IGlobalCommandService globalCommandService, CommandManager manager)
        {
            this.commandManager = manager;
        }

        public Guid Token { get; set; }

        public ITabPanelContent InitializedViewModel(Guid token, string path)
        {
            this.path = path;
            this.Token = token;
            this.navigationCommand = (NavigationInvoker)commandManager.CommandRegister(token, new NavigationInvoker());
            return this;
        }

        public Guid GetPanelToken()
        {
            return this.Token;
        }

        public string GetCurrentPath()
        {
            return path;
        }
    }
}
