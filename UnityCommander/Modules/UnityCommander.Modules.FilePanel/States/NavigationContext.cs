
using System.Collections.ObjectModel;
using UnityCommander.Common.Commands;
using UnityCommander.Core.Navigation;

namespace UnityCommander.Modules.FilePanel.States
{
    public class NavigationContext
    {
        public NavigationManager Navigation { get; set; }

        public ObservableCollection<UICommand> Commands { get; set; }

        public string CurrentPath { get; set; }

        public bool CanGoBack => Navigation.CanGoBack;

        public bool CanGoForward => Navigation.CanGoForward;
    }
}
