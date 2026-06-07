
using System.Collections.ObjectModel;
using UnityCommander.Modules.FilePanel.ViewModels;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public interface IContextMenuHost
    {
        ObservableCollection<MenuItemViewModel>
            ContextMenuItems
        { get; }
    }
}
