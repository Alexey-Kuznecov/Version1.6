
using System.Collections.ObjectModel;
using UnityCommander.Common.Commands;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.States
{
    public class NavigationNodeContext
    {
        public NavigationManager Navigation { get; set; }

        public ObservableCollection<UICommand> Commands { get; set; }

        public ISelectionManager SelectionManager { get; set; }

        public string CurrentPath { get; set; }

        public bool CanGoBack => Navigation.CanGoBack;

        public bool CanGoForward => Navigation.CanGoForward;

        public bool CanGoParent => Navigation.CanGoParent;
       
        public bool CanExecute => true;

    }
}
