using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Core.Modules;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.Viewer.ViewModels
{
    public class ViewerViewModel : BindableBase, ITabPanelContent
    {
        public ViewerViewModel(IPluginLoaderService pluginLoaderService, IGlobalCommandService globalCommandService)
        {

        }

        public Guid Token { get; set; }

        public ITabPanelContent InitializedViewModel(Guid token, string path)
        {
            return null;
        }

        public Guid GetPanelToken()
        {
            return Guid.NewGuid();
        }

        public string GetCurrentPath()
        {
            return null;
        }
    }
}
